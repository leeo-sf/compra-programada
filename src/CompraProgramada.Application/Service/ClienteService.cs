using CompraProgramada.Application.Contract.Service;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;
using CompraProgramada.Shared.Exceptions;
using CompraProgramada.Shared.Request;
using CompraProgramada.Shared.Response;
using OperationResult;

namespace CompraProgramada.Application.Service;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _clienteRepository;
    private readonly ICestaRecomendadaService _cestaRecomendadaService;
    private readonly ICotacaoService _cotacaoService;

    public ClienteService(IClienteRepository clienteRepository,
        ICestaRecomendadaService cestaRecomendadaService,
        ICotacaoService cotacaoService)
    {
        _clienteRepository = clienteRepository;
        _cestaRecomendadaService = cestaRecomendadaService;
        _cotacaoService = cotacaoService;
    }

    public async Task<Result<List<Cliente>>> ObtemClientesAtivoAsync(CancellationToken cancellationToken)
        => await _clienteRepository.ObterClientesAtivosAsync(cancellationToken);

    public async Task<Result<Cliente>> RealizarAdesaoAsync(AdesaoRequest request, CancellationToken cancellationToken)
    {
        var clienteExistente = await _clienteRepository.ExisteAsync(request.Cpf, cancellationToken);
        if (clienteExistente)
            throw new CpfExistenteException();

        var cestaVigente = await _cestaRecomendadaService.ObterCestaAtivaAsync(cancellationToken);
        if (!cestaVigente.IsSuccess)
            return cestaVigente.Exception;

        var cliente = Cliente.Criar(request);

        var clienteSalvo = await _clienteRepository.CriarAsync(cliente, cancellationToken);

        var conta = ContaGrafica.Gerar(clienteSalvo);

        var contaSalva = await _clienteRepository.CriarContaAsync(conta, cancellationToken);

        cliente.AdicionarConta(contaSalva);

        return cliente;
    }

    public async Task<Result<int>> QuantidadeClientesAtivosAsync(CancellationToken cancellationToken)
        => await _clienteRepository.QuantidadeAtivosAsync(cancellationToken);

    public async Task<Result<Cliente>> SairDoProdutoAsync(int clienteId, CancellationToken cancellationToken)
    {
        var cliente = await IdentificarCliente(clienteId, cancellationToken);

        if (!cliente.Ativo)
            return new ApplicationException("Cliente já está com status inativo");

        cliente.Desativar();

        var clienteAtualizado = await _clienteRepository.AtualizarClienteAsync(cliente, cancellationToken);

        return clienteAtualizado;
    }

    public async Task<Result<Cliente>> AtualizarValorMensalAsync(AtualizarValorMensalRequest request, CancellationToken cancellationToken)
    {
        var cliente = await IdentificarCliente(request.ClienteId, cancellationToken);

        if (!cliente.Ativo)
            return new ApplicationException("Cliente com status inativo");

        cliente.AtualizarValorMensal(request);

        var clienteAtualizado = await _clienteRepository.AtualizarClienteAsync(cliente, cancellationToken);

        return clienteAtualizado;
    }

    public async Task<Result<CarteiraCustodiaResponse>> ConsultarCarteiraAsync(int clienteId, CancellationToken cancellationToken)
    {
        var cliente = await IdentificarCliente(clienteId, cancellationToken);

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
        var cliente = await IdentificarCliente(clienteId, cancellationToken);

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

    public async Task<Result<List<ContaGrafica>>> AtualizarContasAsync(List<ContaGrafica> contasAhSeremAtualizadas, CancellationToken cancellationToken)
    {
        if (!contasAhSeremAtualizadas.Any())
            return new ApplicationException("Nenhuma conta gráfica informada para atualização.");

        var contasSalvas = await _clienteRepository.AtualizarContasAsync(contasAhSeremAtualizadas, cancellationToken);

        return contasSalvas;
    }

    private async Task<Cliente> IdentificarCliente(int id, CancellationToken cancellationToken)
    {
        var cliente = await _clienteRepository.ObterClienteAsync(id, cancellationToken);

        if (cliente is not null)
            return cliente;

        throw new ClienteNaoEncontradoException();
    }
}