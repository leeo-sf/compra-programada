using CompraProgramada.Domain.Contract.Handler;
using CompraProgramada.Domain.Contract.Repository;
using CompraProgramada.Domain.Contract.Service;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Mapper;
using CompraProgramada.Shared.Exceptions;
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
        _logger.LogInformation("Iniciando processo de adesão para o cliente {Nome}", request.Nome);

        var clienteExistente = await _clienteRepository.ExisteAsync(request.Cpf, cancellationToken);
        if (clienteExistente)
            return new CpfExistenteException();

        var temCestaRecomendada = await _cestaRecomendadaRepository.ObterCestaAtivaAsync(cancellationToken) is not null;
        if (!temCestaRecomendada)
        {
            _logger.LogWarning("Adesão não pode ser realizada, pois não há cesta recomendada ativa");
            return new AppException("Adesão não pode ser realizada", "CESTA_NAO_ENCONTRADA");
        }

        var cliente = Cliente.Criar(request);

        var clienteSalvo = await _clienteRepository.CriarAsync(cliente, cancellationToken);

        var conta = ContaGrafica.Gerar(clienteSalvo);

        var contaSalva = await _clienteRepository.CriarContaAsync(conta, cancellationToken);

        cliente.AdicionarConta(contaSalva);

        _logger.LogInformation("Adesão realizada com sucesso para o cliente {Nome} com Id {Id}.", cliente.Nome, cliente.Id);

        return _mapper.ToAdesaoResponse(cliente);
    }

    public async Task<Result<SaidaProdutoResponse>> Handle(SaidaProdutoRequest request, CancellationToken cancellationToken)
    {
        var clienteResult = await IdentificarCliente(request.ClienteId, cancellationToken);
        if (!clienteResult.IsSuccess)
            return clienteResult.Exception;

        var cliente = clienteResult.Value;

        _logger.LogInformation("Iniciando processo de saida do produto para o cliente {ClienteId}.", request.ClienteId);

        if (cliente is { Ativo: false })
        {
            _logger.LogWarning("Cliente {ClienteId} já está inativo.", request.ClienteId);
            return new AppException("Cliente já está inativo", "CLIENTE_INATIVO");
        }

        cliente.Desativar();

        var clienteAtualizado = await _clienteRepository.AtualizarClienteAsync(cliente, cancellationToken);

        _logger.LogInformation("Solicitação de saída do produto realizada com sucesso.");

        return _mapper.ToSaidaProdutoResponse(clienteAtualizado);
    }

    public async Task<Result<AtualizarValorMensalResponse>> Handle(AtualizarValorMensalRequest request, CancellationToken cancellationToken)
    {
        var clienteResult = await IdentificarCliente(request.ClienteId, cancellationToken);
        if (!clienteResult.IsSuccess)
            return clienteResult.Exception;

        _logger.LogInformation("Solicitação do ClienteId: {ClienteId} para alteração do valor mensal, novo valor mensal: {NovoValorMensal}", request.ClienteId, request.NovoValorMensal);

        var cliente = clienteResult.Value;

        if (cliente is { Ativo: false })
            return new AppException("Cliente já está inativo", "CLIENTE_INATIVO");

        cliente.AtualizarValorMensal(request);

        var clienteAtualizado = await _clienteRepository.AtualizarClienteAsync(cliente, cancellationToken);

        _logger.LogInformation("Valor mensal do ClienteId {ClientId} atualizado para: {NovoValor}", clienteAtualizado.Id, clienteAtualizado.ValorMensal);

        return _mapper.ToAtualizarValorMensalResponse(cliente);
    }

    public async Task<Result<CarteiraCustodiaResponse>> Handle(CarteiraCustodiaRequest request, CancellationToken cancellationToken)
    {
        var clienteResult = await IdentificarCliente(request.ClienteId, cancellationToken);
        if (!clienteResult.IsSuccess)
            return clienteResult.Exception;

        var cliente = clienteResult.Value;

        _logger.LogInformation("Cliente solicitando consulta da carteira: {ClienteId}", request);

        var cestaVigente = await _cestaRecomendadaRepository.ObterCestaAtivaAsync(cancellationToken);
        if (cestaVigente is null)
            return new AppException("Nenhuma cesta vigente encontrada", "CESTA_NAO_ENCONTRADA");

        var cotacaoResult = await _cotacaoService.ObterCotacoesFechamentoB3DaCestaRecomendadaAsync(cestaVigente, cancellationToken);
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

        _logger.LogInformation("Cliente solicitando consulta da rentabilidade da carteira: {ClienteId}", request);

        var cestaVigente = await _cestaRecomendadaRepository.ObterCestaAtivaAsync(cancellationToken);
        if (cestaVigente is null)
            return new AppException("Nenhuma cesta vigente encontrada", "CESTA_NAO_ENCONTRADA");

        var fechamento = await _cotacaoService.ObterCotacoesFechamentoB3DaCestaRecomendadaAsync(cestaVigente, cancellationToken);
        if (!fechamento.IsSuccess)
            return fechamento.Exception;

        var resumoRentabilidade = cliente.ContaGrafica.CalcularResumoDeRentabilidade(fechamento.Value);
        var historicoAportes = cliente.ContaGrafica.HistoricoAportes();
        var evolucaoCarteira = cliente.ContaGrafica.CalcularEvolucaoCarteira(fechamento.Value);

        return new RentabilidadeResponse(cliente.Id, cliente.Nome, DateTime.Now, resumoRentabilidade, historicoAportes, evolucaoCarteira);
    }

    private async Task<Result<Cliente>> IdentificarCliente(int id, CancellationToken cancellationToken)
    {
        var cliente = await _clienteRepository.ObterClienteAsync(id, cancellationToken);

        if (cliente is not null)
            return cliente;

        return new ClienteNaoEncontradoException();
    }
}