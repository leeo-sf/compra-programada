using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Interface;
using CompraProgramada.Application.Mapper;
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
    private readonly ClienteMapper _mapper;

    public ClienteService(IClienteRepository clienteRepository,
        IContaGraficaService contaService,
        ICestaRecomendadaService cestaRecomendadaService,
        ClienteMapper mapper)
    {
        _clienteRepository = clienteRepository;
        _contaService = contaService;
        _cestaRecomendadaService = cestaRecomendadaService;
        _mapper = mapper;
    }

    public async Task<Result<List<Cliente>>> ObtemClientesAtivoAsync(CancellationToken cancellationToken)
    {
        var clientes = await _clienteRepository.ObterClientesAtivosAsync(cancellationToken);

        return clientes;
    }

    public async Task<Result<ClienteDto>> RealizarAdesaoAsync(AdesaoRequest request, CancellationToken cancellationToken)
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

        return _mapper.ToResponse(cliente);
    }

    public async Task<Result<int>> QuantidadeClientesAtivosAsync(CancellationToken cancellationToken)
    {
        var quantidade = await _clienteRepository.QuantidadeAtivosAsync(cancellationToken);

        return quantidade;
    }

    public async Task<Result<ClienteDto>> SairDoProdutoAsync(int clienteId, CancellationToken cancellationToken)
    {
        var cliente = await _clienteRepository.ObterClienteAsync(clienteId, cancellationToken);

        if (cliente is null)
            throw new ClienteNaoEncontradoException();

        if (!cliente.Ativo)
            return new ApplicationException("Cliente já está com status inativo");

        cliente.Desativar();

        var clienteAtualizado = await _clienteRepository.AtualizarClienteAsync(cliente, cancellationToken);

        return _mapper.ToResponse(clienteAtualizado);
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

        return _mapper.ToResponse(clienteAtualizado);
    }

    public async Task<Result<CarteiraCustodiaResponse>> ConsultarCarteiraAsync(int clienteId, CancellationToken cancellationToken)
    {
        var cliente = await _clienteRepository.ObterClienteAsync(clienteId, cancellationToken);
        if (cliente is null)
            throw new ClienteNaoEncontradoException();

        var custodiasDto = cliente.ContaGrafica!.CustodiaFilhotes
            .Select(x => new CustodiaFilhoteDto(x.Id, x.ContaGraficaId, x.Ticker!, x.PrecoMedio, x.Quantidade)).ToList();

        var resumoRentabilidadeResult = await _contaService.ObterRentabilidadeDaCertira(custodiasDto, cancellationToken);
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

        var rentabilidadeResult = await _contaService.ObterRentabilidadeDaCertira(contaDto.CustodiaFilhotes, cancellationToken);
        if (!rentabilidadeResult.IsSuccess)
            return rentabilidadeResult.Exception;

        var result = await _contaService.ObterEvolucaoDaCertira(contaDto, cancellationToken);
        if (!result.IsSuccess)
            return result.Exception;

        return new RentabilidadeResponse(cliente.Id, cliente.Nome, DateTime.Now, rentabilidadeResult.Value.Resumo, result.Value.HistoricoAportes, result.Value.EvolucaoCarteira);
    }
}