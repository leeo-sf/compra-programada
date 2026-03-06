using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Exceptions;
using CompraProgramada.Application.Interface;
using CompraProgramada.Application.Request;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;
using OperationResult;

namespace CompraProgramada.Application.Service;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IContaGraficaService _contaService;
    private readonly ICestaRecomendadaService _cestaRecomendadaService;
    private const string VALOR_MENSAL_MINIMO_MENSAGEM = "O valor mensal minimo e de R$ 100,00.";
    private const string VALOR_MENSAL_MINIMO_CODIGO = "VALOR_MENSAL_INVALIDO";
    private const string CPF_CADASTRADO_MENSAGEM = "CPF ja cadastrado no sistema.";
    private const string CPF_CADASTRADO_CODIGO = "CLIENTE_CPF_DUPLICADO";

    public ClienteService(IClienteRepository clienteRepository,
        IContaGraficaService contaService,
        ICestaRecomendadaService cestaRecomendadaService)
    {
        _clienteRepository = clienteRepository;
        _contaService = contaService;
        _cestaRecomendadaService = cestaRecomendadaService;
    }

    public async Task<Result<List<ClienteDto>>> ObtemClientesAtivoAsync(CancellationToken cancellationToken)
    {
        var clientes = await _clienteRepository.ObterClientesAtivosAsync(cancellationToken);

        return clientes.Select(c => GerarClienteDto(c)).ToList();
    }

    public async Task<Result<ClienteDto>> AdesaoAsync(AdesaoRequest request, CancellationToken cancellationToken)
    {
        if (request.ValorMensal < 100)
            return new ErroMapeadoException(VALOR_MENSAL_MINIMO_MENSAGEM, VALOR_MENSAL_MINIMO_CODIGO);

        var clienteExistente = await _clienteRepository.ExisteAsync(request.Cpf, cancellationToken);

        if (clienteExistente)
            return new ErroMapeadoException(CPF_CADASTRADO_MENSAGEM, CPF_CADASTRADO_CODIGO);

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