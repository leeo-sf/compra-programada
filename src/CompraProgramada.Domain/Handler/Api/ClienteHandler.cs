using CompraProgramada.Domain.Contract.Handler;
using CompraProgramada.Domain.Contract.Repository;
using CompraProgramada.Domain.Contract.Service;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Mapper;
using CompraProgramada.Shared.Exceptions;
using CompraProgramada.Shared.Extensions;
using CompraProgramada.Shared.Request;
using CompraProgramada.Shared.Response;
using MediatR;
using Microsoft.Extensions.Logging;
using OperationResult;

namespace CompraProgramada.Domain.Handler.Api;

public class ClienteHandle
    : IRequestHandler<AdesaoRequest, Result<AdesaoResponse>>,
        IRequestHandler<SaidaProdutoRequest, Result<SaidaProdutoResponse>>,
        IRequestHandler<AtualizarValorMensalRequest, Result<AtualizarValorMensalResponse>>,
        IRequestHandler<CarteiraCustodiaRequest, Result<CarteiraCustodiaResponse>>,
        IRequestHandler<RentabilidadeRequest, Result<RentabilidadeResponse>>, IApiRequestHandler
{
    private readonly ILogger<ClienteHandle> _logger;
    private readonly IClienteRepository _clienteRepository;
    private readonly ICestaRecomendadaRepository _cestaRecomendadaRepository;
    private readonly ICotacaoService _cotacaoService;
    private readonly ClienteMapper _mapper;

    public ClienteHandle(
        ILogger<ClienteHandle> logger,
        IClienteRepository clienteRepository,
        ICestaRecomendadaRepository cestaRecomendadaRepository,
        ICotacaoService cotacaoService,
        ClienteMapper mapper)
    {
        _logger = logger;
        _clienteRepository = clienteRepository;
        _cestaRecomendadaRepository = cestaRecomendadaRepository;
        _cotacaoService = cotacaoService;
        _mapper = mapper;
    }

    public async Task<Result<AdesaoResponse>> Handle(AdesaoRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInicioProcessoAdesao(request.Nome);

        var clienteExistente = await _clienteRepository.CpfExistenteAsync(request.Cpf, cancellationToken);
        if (clienteExistente)
            return new CpfExistenteException();

        var temCestaRecomendada = await _cestaRecomendadaRepository.ObterCestaAtualAsync(cancellationToken) is not null;
        if (!temCestaRecomendada)
        {
            _logger.LogFalhaAdesaoCestaNaoAtiva();
            return new AppException("Adesão não pode ser realizada", "CESTA_NAO_ENCONTRADA");
        }

        var cliente = Cliente.Criar(request);

        await _clienteRepository.CriarAsync(cliente, cancellationToken);

        var conta = ContaGrafica.Gerar(cliente);

        await _clienteRepository.CriarContaAsync(conta, cancellationToken);

        cliente.AdicionarConta(conta);

        _logger.LogAdesaoRealizadaComSucesso(cliente.Nome, cliente.Id);

        return _mapper.ToAdesaoResponse(cliente);
    }

    public async Task<Result<SaidaProdutoResponse>> Handle(SaidaProdutoRequest request, CancellationToken cancellationToken)
    {
        var clienteResult = await IdentificarCliente(request.ClienteId, cancellationToken);
        if (!clienteResult.IsSuccess)
            return clienteResult.Exception;

        var cliente = clienteResult.Value;

        _logger.LogInicioProcessoSaidaProduto(request.ClienteId);

        if (cliente is { Ativo: false })
        {
            _logger.LogClienteInativo(request.ClienteId);
            return new AppException("Cliente já está inativo", "CLIENTE_INATIVO");
        }

        cliente.Desativar();

        await _clienteRepository.AtualizarAsync(cliente, cancellationToken);

        _logger.LogSaidaProdutoRealizadaComSucesso();

        return _mapper.ToSaidaProdutoResponse(cliente);
    }

    public async Task<Result<AtualizarValorMensalResponse>> Handle(AtualizarValorMensalRequest request, CancellationToken cancellationToken)
    {
        var clienteResult = await IdentificarCliente(request.ClienteId, cancellationToken);
        if (!clienteResult.IsSuccess)
            return clienteResult.Exception;

        _logger.LogInicioAtualizacaoValorMensal(request.ClienteId, request.NovoValorMensal);

        var cliente = clienteResult.Value;

        if (cliente is { Ativo: false })
            return new AppException("Cliente já está inativo", "CLIENTE_INATIVO");

        cliente.AtualizarValorMensal(request);

        await _clienteRepository.AtualizarAsync(cliente, cancellationToken);

        _logger.LogValorMensalAtualizado(cliente.Id, cliente.ValorMensal);

        return _mapper.ToAtualizarValorMensalResponse(cliente);
    }

    public async Task<Result<CarteiraCustodiaResponse>> Handle(CarteiraCustodiaRequest request, CancellationToken cancellationToken)
    {
        var clienteResult = await IdentificarCliente(request.ClienteId, cancellationToken);
        if (!clienteResult.IsSuccess)
            return clienteResult.Exception;

        var cliente = clienteResult.Value;

        if (cliente.ContaGrafica is { CarteiraAtiva: false })
            return new AppException("Cliente não tem uma carteira ativa", "CARTEIRA_INATIVA");

        _logger.LogConsultaCarteira(request.ClienteId);

        var cestaVigente = await _cestaRecomendadaRepository.ObterCestaAtualAsync(cancellationToken);
        if (cestaVigente is null)
            return new AppException("Nenhuma cesta vigente encontrada", "CESTA_NAO_ENCONTRADA");

        var cotacaoResult = await _cotacaoService.ObterCotacoesDaCestaRecomendadaAsync(cestaVigente, cancellationToken);
        if (!cotacaoResult.IsSuccess)
            return cotacaoResult.Exception;

        var resumoRentabilidade = cliente.ContaGrafica.CalcularResumoDeRentabilidade(cotacaoResult.Value);
        var detalhesCarteira = cliente.ContaGrafica.CalcularDetalhesCarteira(cotacaoResult.Value, resumoRentabilidade.ValorAtualCarteira);

        return new CarteiraCustodiaResponse(cliente.Id, cliente.Nome, cliente.ContaGrafica.NumeroConta, DateTime.Now, resumoRentabilidade, detalhesCarteira);
    }

    public async Task<Result<RentabilidadeResponse>> Handle(RentabilidadeRequest request, CancellationToken cancellationToken)
    {
        var clienteResult = await IdentificarCliente(request.ClienteId, cancellationToken);
        if (!clienteResult.IsSuccess)
            return clienteResult.Exception;

        var cliente = clienteResult.Value;

        if (cliente.ContaGrafica is { CarteiraAtiva: false })
            return new AppException("Cliente não tem uma carteira ativa", "CARTEIRA_INATIVA");

        _logger.LogConsultaRentabilidade(request.ClienteId);

        var cestaVigente = await _cestaRecomendadaRepository.ObterCestaAtualAsync(cancellationToken);
        if (cestaVigente is null)
            return new AppException("Nenhuma cesta vigente encontrada", "CESTA_NAO_ENCONTRADA");

        var fechamento = await _cotacaoService.ObterCotacoesDaCestaRecomendadaAsync(cestaVigente, cancellationToken);
        if (!fechamento.IsSuccess)
            return fechamento.Exception;

        var resumoRentabilidade = cliente.ContaGrafica.CalcularResumoDeRentabilidade(fechamento.Value);
        var historicoAportes = cliente.ContaGrafica.HistoricoAportes();
        var evolucaoCarteira = cliente.ContaGrafica.CalcularEvolucaoCarteira(fechamento.Value);

        return new RentabilidadeResponse(cliente.Id, cliente.Nome, DateTime.Now, resumoRentabilidade, historicoAportes, evolucaoCarteira);
    }

    private async Task<Result<Cliente>> IdentificarCliente(int id, CancellationToken cancellationToken)
    {
        var cliente = await _clienteRepository.ObterAsync(id, cancellationToken);

        if (cliente is not null)
            return cliente;

        return new ClienteNaoEncontradoException();
    }
}