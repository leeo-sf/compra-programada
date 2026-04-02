using CompraProgramada.Application.Mapper;
using CompraProgramada.Shared.Request;
using CompraProgramada.Shared.Response;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Shared.Exceptions;
using CompraProgramada.Domain.Interface;
using OperationResult;
using CompraProgramada.Application.Contract.Service;

namespace CompraProgramada.Application.Service;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IContaGraficaService _contaService;
    private readonly ICestaRecomendadaService _cestaRecomendadaService;
    private readonly ICotacaoService _cotacaoService;
    private readonly ClienteMapper _mapper;

    public ClienteService(IClienteRepository clienteRepository,
        IContaGraficaService contaService,
        ICestaRecomendadaService cestaRecomendadaService,
        ICotacaoService cotacaoService,
        ClienteMapper mapper)
    {
        _clienteRepository = clienteRepository;
        _contaService = contaService;
        _cestaRecomendadaService = cestaRecomendadaService;
        _cotacaoService = cotacaoService;
        _mapper = mapper;
    }

    public async Task<Result<List<Cliente>>> ObtemClientesAtivoAsync(CancellationToken cancellationToken)
        => await _clienteRepository.ObterClientesAtivosAsync(cancellationToken);

    public async Task<Result<Cliente>> RealizarAdesaoAsync(AdesaoRequest request, CancellationToken cancellationToken)
    {
        var clienteExistente = await _clienteRepository.ExisteAsync(request.Cpf, cancellationToken);
        if (clienteExistente)
            return new CpfExistenteException();

        var cestaVigente = await _cestaRecomendadaService.ObterCestaAtivaAsync(cancellationToken);
        if (!cestaVigente.IsSuccess)
            return cestaVigente.Exception;

        var cliente = Cliente.Criar(request);

        var clienteSalvo = await _clienteRepository.CriarAsync(cliente, cancellationToken);

        var contaSalva = await _contaService.GerarContaGraficaAsync(clienteSalvo, cancellationToken);
        if (!contaSalva.IsSuccess)
            return contaSalva.Exception;

        cliente.AdicionarConta(contaSalva.Value);

        return cliente;
    }

    public async Task<Result<int>> QuantidadeClientesAtivosAsync(CancellationToken cancellationToken)
        => await _clienteRepository.QuantidadeAtivosAsync(cancellationToken);

    public async Task<Result<Cliente>> SairDoProdutoAsync(int clienteId, CancellationToken cancellationToken)
    {
        var cliente = await _clienteRepository.ObterClienteAsync(clienteId, cancellationToken);

        if (cliente is null)
            return new ClienteNaoEncontradoException();

        if (!cliente.Ativo)
            return new ApplicationException("Cliente já está com status inativo");

        cliente.Desativar();

        var clienteAtualizado = await _clienteRepository.AtualizarClienteAsync(cliente, cancellationToken);

        return clienteAtualizado;
    }

    public async Task<Result<Cliente>> AtualizarValorMensalAsync(AtualizarValorMensalRequest request, CancellationToken cancellationToken)
    {
        var cliente = await _clienteRepository.ObterClienteAsync(request.ClienteId, cancellationToken);

        if (cliente is null)
            return new ClienteNaoEncontradoException();

        if (!cliente.Ativo)
            return new ApplicationException("Cliente com status inativo");

        cliente.AtualizarValorMensal(request);

        var clienteAtualizado = await _clienteRepository.AtualizarClienteAsync(cliente, cancellationToken);

        return clienteAtualizado;
    }

    public async Task<Result<CarteiraCustodiaResponse>> ConsultarCarteiraAsync(int clienteId, CancellationToken cancellationToken)
    {
        var cliente = await _clienteRepository.ObterClienteAsync(clienteId, cancellationToken);
        if (cliente is null)
            throw new ClienteNaoEncontradoException();

        var cestaVigenteResult = await _cestaRecomendadaService.ObterCestaAtivaAsync(cancellationToken);
        if (!cestaVigenteResult.IsSuccess)
            return cestaVigenteResult.Exception;

        var cotacaoResult = await _cotacaoService.ObterCotacoesFechamentoB3DaCestaRecomendadaAsync(cestaVigenteResult.Value, cancellationToken);
        if (!cotacaoResult.IsSuccess)
            return cotacaoResult.Exception;

        var resumoRentabilidade = cliente.ContaGrafica.CalcularResumoDeRentabilidade(cotacaoResult.Value);

        var detalhesCarteira = cliente.ContaGrafica.CalcularDetalhesCarteira(cotacaoResult.Value, resumoRentabilidade.ValorAtualCarteira);

        return new CarteiraCustodiaResponse(cliente.Id, cliente.Nome, cliente.ContaGrafica.NumeroConta, DateTime.Now, resumoRentabilidade, detalhesCarteira);
    }

    public async Task<Result<RentabilidadeResponse>> ConsultarRentabilidadeAsync(int clienteId, CancellationToken cancellationToken)
    {
        var cliente = await _clienteRepository.ObterClienteAsync(clienteId, cancellationToken);
        if (cliente is null)
            throw new ClienteNaoEncontradoException();

        var cestaVigente = await _cestaRecomendadaService.ObterCestaAtivaAsync(cancellationToken);
        if (!cestaVigente.IsSuccess)
            return cestaVigente.Exception;

        var fechamento = await _cotacaoService.ObterCotacoesFechamentoB3DaCestaRecomendadaAsync(cestaVigente.Value, cancellationToken);
        if (!fechamento.IsSuccess)
            return fechamento.Exception;

        var resumoRentabilidade = cliente.ContaGrafica.CalcularResumoDeRentabilidade(fechamento.Value);

        var historicoAportes = cliente.ContaGrafica.HistoricoAportes();

        var evolucaoCarteira = cliente.ContaGrafica.CalcularEvolucaoCarteira(fechamento.Value);

        return new RentabilidadeResponse(cliente.Id, cliente.Nome, DateTime.Now, resumoRentabilidade, historicoAportes, evolucaoCarteira);
    }
}