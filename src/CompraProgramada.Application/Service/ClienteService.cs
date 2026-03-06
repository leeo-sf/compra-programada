using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Exceptions;
using CompraProgramada.Application.Interface;
using CompraProgramada.Application.Request;
using CompraProgramada.Application.Response;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;
using OperationResult;
using System.Net;

namespace CompraProgramada.Application.Service;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IContaGraficaService _contaService;
    private readonly ICestaRecomendadaService _cestaRecomendadaService;
    private readonly ICustodiaFilhoteService _custodiaFilhoteService;
    private const string VALOR_MENSAL_MINIMO_CODIGO = "VALOR_MENSAL_INVALIDO";
    private const string CPF_CADASTRADO_CODIGO = "CLIENTE_CPF_DUPLICADO";
    private const string CLIENTE_NAO_ENCONTRADO_CODIGO = "CLIENTE_NAO_ENCONTRADO";
    private const string CLIENTE_INATIVO_CODIGO = "CLIENTE_INATIVO";
    private const decimal VALOR_MINIMO_ADESAO = 100;

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
        if (request.ValorMensal < VALOR_MINIMO_ADESAO)
            return new ErroMapeadoException($"O valor mensal minimo e de R$ {VALOR_MINIMO_ADESAO.ToString("F2")}", VALOR_MENSAL_MINIMO_CODIGO);

        var clienteExistente = await _clienteRepository.ExisteAsync(request.Cpf, cancellationToken);

        if (clienteExistente)
            return new ErroMapeadoException("CPF ja cadastrado no sistema.", CPF_CADASTRADO_CODIGO);

        var cestaVigente = await _cestaRecomendadaService.ObterCestaAtivaAsync(cancellationToken);

        if (!cestaVigente.IsSuccess)
            return cestaVigente.Exception;

        var cliente = new Cliente(
            0,
            request.Nome,
            request.Cpf,
            request.Email,
            request.ValorMensal,
            request.ValorMensal,
            DateTime.UtcNow);

        var clienteSalvo = await _clienteRepository.CriarAsync(cliente, cancellationToken);

        var contaSalva = await _contaService.GerarContaGraficaAsync(new ContaGraficaDto
            {
                Id = 0,
                DataCriacao = DateTime.Now,
                ClienteId = clienteSalvo.Id
            }, cancellationToken);

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
            return new ErroMapeadoException("Cliente nao encontrado.", CLIENTE_NAO_ENCONTRADO_CODIGO, HttpStatusCode.NotFound);

        if (!cliente.Ativo)
            return new ErroMapeadoException("Cliente já está com status inativo", CLIENTE_INATIVO_CODIGO);

        var dadosAtualizadosCliente = cliente with { Ativo = false };

        var clienteAtualizado = await _clienteRepository.AtualizarClienteAsync(cliente, dadosAtualizadosCliente, cancellationToken);

        return GerarClienteDto(cliente);
    }

    public async Task<Result<ClienteDto>> AtualizarValorMensalAsync(AtualizarValorMensalRequest request, CancellationToken cancellationToken)
    {
        if (request.NovoValorMensal < VALOR_MINIMO_ADESAO)
            return new ErroMapeadoException("O valor mensal minimo e de R$ 100,00.", VALOR_MENSAL_MINIMO_CODIGO);

        var cliente = await _clienteRepository.ObterClienteAsync(request.ClienteId, cancellationToken);

        if (cliente is null)
            return new ErroMapeadoException("Cliente nao encontrado.", CLIENTE_NAO_ENCONTRADO_CODIGO, HttpStatusCode.NotFound);

        if (!cliente.Ativo)
            return new ErroMapeadoException("Cliente com status inativo", CLIENTE_INATIVO_CODIGO);

        var dadosAtualizadosCliente = cliente with { ValorMensal = request.NovoValorMensal };

        var clienteAtualizado = await _clienteRepository.AtualizarClienteAsync(cliente, dadosAtualizadosCliente, cancellationToken);

        return GerarClienteDto(clienteAtualizado);
    }

    public async Task<Result<CarteiraCustodiaResponse>> ConsultarCarteiraAsync(int clienteId, CancellationToken cancellationToken)
    {
        var cliente = await _clienteRepository.ObterClienteAsync(clienteId, cancellationToken);
        if (cliente is null)
            return new ErroMapeadoException("Cliente nao encontrado.", CLIENTE_NAO_ENCONTRADO_CODIGO, HttpStatusCode.NotFound);

        var custodiasDto = cliente.ContaGrafica!.CustodiaFilhotes
            .Select(x => new CustodiaFilhoteDto(x.Id, x.ContaGraficaId, x.Ticker!, x.PrecoMedio, x.Quantidade)).ToList();

        var carteiraResult = await _custodiaFilhoteService.ObterRentabilidadeDaCertira(custodiasDto);
        if (!carteiraResult.IsSuccess)
            return new ApplicationException("Falha ao obter detalhes da carteira.");

        var carteiraValue = carteiraResult.Value;

        return new CarteiraCustodiaResponse(cliente.Id, cliente.Nome, cliente.ContaGrafica.NumeroConta, carteiraValue.Resumo, carteiraValue.Ativos);
    }

    public ClienteDto GerarClienteDto(Cliente cliente)
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
            ContaGrafica = new ContaGraficaDto
            {
                Id = cliente.ContaGrafica!.Id,
                NumeroConta = cliente.ContaGrafica.NumeroConta,
                DataCriacao = cliente.ContaGrafica.DataCriacao,
                ClienteId = cliente.ContaGrafica.Id,
                Tipo = cliente.ContaGrafica.Tipo,
                CustodiaFilhote = cliente.ContaGrafica.CustodiaFilhotes.Select(cf => new CustodiaFilhoteDto
                {
                    Id = cf.Id,
                    ContaGraficaId = cf.ContaGraficaId,
                    Ticker = cf.Ticker ?? string.Empty,
                    PrecoMedio = cf.PrecoMedio,
                    Quantidade = cf.Quantidade
                }).ToList()
            }
        };
}