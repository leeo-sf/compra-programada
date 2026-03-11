using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Interface;
using CompraProgramada.Application.Request;
using CompraProgramada.Application.Response;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Exceptions;
using CompraProgramada.Domain.Interface;
using OperationResult;

namespace CompraProgramada.Application.Service;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IContaGraficaService _contaService;
    private readonly ICestaRecomendadaService _cestaRecomendadaService;
    private readonly ICustodiaFilhoteService _custodiaFilhoteService;

    public ClienteService(IClienteRepository clienteRepository,
        IContaGraficaService contaService,
        ICestaRecomendadaService cestaRecomendadaService,
        ICustodiaFilhoteService custodiaFilhoteService)
    {
        _clienteRepository = clienteRepository;
        _contaService = contaService;
        _cestaRecomendadaService = cestaRecomendadaService;
        _custodiaFilhoteService = custodiaFilhoteService;
    }

    public async Task<Result<List<ClienteDto>>> ObtemClientesAtivoAsync(CancellationToken cancellationToken)
    {
        var clientes = await _clienteRepository.ObterClientesAtivosAsync(cancellationToken);

        return clientes.Select(c => GerarClienteDto(c)).ToList();
    }

    public async Task<Result<ClienteDto>> AdesaoAsync(AdesaoRequest request, CancellationToken cancellationToken)
    {
        var clienteExistente = await _clienteRepository.ExisteAsync(request.Cpf, cancellationToken);
        if (clienteExistente)
            throw new CpfExistenteException();

        var cestaVigente = await _cestaRecomendadaService.ObterCestaAtivaAsync(cancellationToken);
        if (!cestaVigente.IsSuccess)
            return cestaVigente.Exception;

        var cliente = Cliente.Criar(request.Nome, request.Cpf, request.Email, request.ValorMensal);

        var clienteSalvo = await _clienteRepository.CriarAsync(cliente, cancellationToken);

        var contaSalva = await _contaService.GerarContaGraficaAsync(clienteSalvo.Id, cancellationToken);
        if (!contaSalva.IsSuccess)
            return contaSalva.Exception;

        return GerarClienteDto(cliente);
    }

    public async Task<Result<int>> QuantidadeAtivosAsync(CancellationToken cancellationToken)
    {
        var quantidade = await _clienteRepository.QuantidadeAtivosAsync(cancellationToken);

        return quantidade;
    }

    public Result<decimal> TotalConsolidade(List<ClienteDto> clientesAtivos)
        => clientesAtivos.Sum(cliente => cliente.ValorMensal / 3);

    public async Task<Result<ClienteDto>> SairDoProdutoAsync(int clienteId, CancellationToken cancellationToken)
    {
        var cliente = await _clienteRepository.ObterClienteAsync(clienteId, cancellationToken);

        if (cliente is null)
            throw new ClienteNaoEncontradoException();

        if (!cliente.Ativo)
            return new ApplicationException("Cliente já está com status inativo");

        cliente.Desativar();

        var clienteAtualizado = await _clienteRepository.AtualizarClienteAsync(cliente, cancellationToken);

        return GerarClienteDto(cliente);
    }

    public async Task<Result<ClienteDto>> AtualizarValorMensalAsync(AtualizarValorMensalRequest request, CancellationToken cancellationToken)
    {
        var cliente = await _clienteRepository.ObterClienteAsync(request.ClienteId, cancellationToken);

        if (cliente is null)
            throw new ClienteNaoEncontradoException();

        if (!cliente.Ativo)
            return new ApplicationException("Cliente com status inativo");

        cliente.AtualizarValorMensal(request.NovoValorMensal);

        var clienteAtualizado = await _clienteRepository.AtualizarClienteAsync(cliente, cancellationToken);

        return GerarClienteDto(clienteAtualizado);
    }

    public async Task<Result<CarteiraCustodiaResponse>> ConsultarCarteiraAsync(int clienteId, CancellationToken cancellationToken)
    {
        var cliente = await _clienteRepository.ObterClienteAsync(clienteId, cancellationToken);
        if (cliente is null)
            throw new ClienteNaoEncontradoException();

        var custodiasDto = cliente.ContaGrafica!.CustodiaFilhotes
            .Select(x => new CustodiaFilhoteDto(x.Id, x.ContaGraficaId, x.Ticker!, x.PrecoMedio, x.Quantidade)).ToList();

        var resumoRentabilidadeResult = await _custodiaFilhoteService.ObterRentabilidadeDaCertira(custodiasDto, cancellationToken);
        if (!resumoRentabilidadeResult.IsSuccess)
            return new ApplicationException("Falha ao obter detalhes da carteira.");

        var resumoRentabilidadeValue = resumoRentabilidadeResult.Value.Resumo;
        var ativosCarteitaValue = resumoRentabilidadeResult.Value.Ativos;

        return new CarteiraCustodiaResponse(cliente.Id, cliente.Nome, cliente.ContaGrafica.NumeroConta, DateTime.Now, resumoRentabilidadeValue, ativosCarteitaValue);
    }

    public async Task<Result<RentabilidadeResponse>> ConsultarRentabilidadeAsync(int clienteId, CancellationToken cancellationToken)
    {
        var cliente = await _clienteRepository.ObterClienteAsync(clienteId, cancellationToken);
        if (cliente is null)
            throw new ClienteNaoEncontradoException();

        if (!cliente!.ContaGrafica.HistoricoCompra.Any())
            return new ApplicationException("Cliente ainda não tem compras realizadas.");

        var conta = cliente.ContaGrafica;
        var contaDto = new ContaGraficaDto(
            conta.Id,
            conta.NumeroConta,
            conta.DataCriacao,
            conta.ClienteId,
            conta.Tipo,
            conta.HistoricoCompra.Select(hc => new HistoricoCompraDto(conta.Id, hc.Ticker, hc.Quantidade, hc.ValorAporte, hc.PrecoExecutado, hc.PrecoMedio, hc.Data)).ToList(),
            conta.CustodiaFilhotes.Select(cf => new CustodiaFilhoteDto(cf.Id, cf.ContaGraficaId, cf.Ticker, cf.PrecoMedio, cf.Quantidade)).ToList());

        var rentabilidadeResult = await _custodiaFilhoteService.ObterRentabilidadeDaCertira(contaDto.CustodiaFilhotes, cancellationToken);
        if (!rentabilidadeResult.IsSuccess)
            return rentabilidadeResult.Exception;

        var result = await _custodiaFilhoteService.ObterEvolucaoDaCertira(contaDto, cancellationToken);
        if (!result.IsSuccess)
            return result.Exception;

        return new RentabilidadeResponse(cliente.Id, cliente.Nome, DateTime.Now, rentabilidadeResult.Value.Resumo, result.Value.HistoricoAportes, result.Value.EvolucaoCarteira);
    }

    private ClienteDto GerarClienteDto(Cliente cliente)
        => new ClienteDto
        {
            ClienteId = cliente.Id,
            Nome = cliente.Nome,
            Cpf = cliente.Cpf,
            Email = cliente.Email,
            ValorAnterior = cliente.ValorAnterior,
            ValorMensal = cliente.ValorMensal,
            Ativo = cliente.Ativo,
            DataAdesao = cliente.DataAdesao,
            ContaGrafica = new ContaGraficaDto(
                cliente.ContaGrafica!.Id,
                cliente.ContaGrafica.NumeroConta,
                cliente.ContaGrafica.DataCriacao,
                cliente.Id,
                cliente.ContaGrafica.Tipo,
                cliente.ContaGrafica.HistoricoCompra.Select(hc => new HistoricoCompraDto(
                    cliente.ContaGrafica.Id,
                    hc.Ticker,
                    hc.Quantidade,
                    hc.ValorAporte,
                    hc.PrecoExecutado,
                    hc.PrecoMedio,
                    hc.Data)).ToList(),
                cliente.ContaGrafica.CustodiaFilhotes.Select(cf => new CustodiaFilhoteDto(
                    cf.Id,
                    cf.ContaGraficaId,
                    cf.Ticker ?? string.Empty,
                    cf.PrecoMedio,
                    cf.Quantidade
                )).ToList()
            )
        };
}