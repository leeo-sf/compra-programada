using CompraProgramada.Application.Exceptions;
using CompraProgramada.Application.Interface;
using CompraProgramada.Application.Request;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;

namespace CompraProgramada.Application.Service;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IContaService _contaService;
    private readonly ICustodiaService _custodiaService;

    public ClienteService(IClienteRepository clienteRepository,
        IContaService contaService,
        ICustodiaService custodiaService)
    {
        _clienteRepository = clienteRepository;
        _contaService = contaService;
        _custodiaService = custodiaService;
    }

    public async Task<Result<List<Cliente>>> ObtemClientesAtivoAsync(CancellationToken cancellationToken)
    {
        var clientes = await _clienteRepository.ObterClientesAtivosAsync(cancellationToken);

        return Result<List<Cliente>>.Ok(clientes);
    }

    public async Task<Result<Cliente>> CadastrarClienteAsync(AdesaoRequest request, CancellationToken cancellationToken)
    {
        if (request.ValorMensal < 100)
            return Result<Cliente>.Fail(new ErroMapeadoException("O valor mensal minimo e de R$ 100,00.", "VALOR_MENSAL_INVALIDO"));
        
        var clienteExistente = await _clienteRepository.ExisteAsync(request.Cpf, cancellationToken);

        if (clienteExistente)
            return Result<Cliente>.Fail(new ErroMapeadoException("CPF ja cadastrado no sistema.", "CLIENTE_CPF_DUPLICADO"));

        var cliente = new Cliente(
            0,
            request.Nome,
            request.Cpf,
            request.Email,
            request.ValorMensal,
            request.ValorMensal,
            DateTime.UtcNow);


        var clienteSalvo = await _clienteRepository.CriarAsync(cliente, cancellationToken);

        var contaSalva = await _contaService.GerarContaGraficaAsync(clienteSalvo, cancellationToken);

        var custodiaFilhoteSalva = await _custodiaService.GerarCustodiaFilhoteAsync(contaSalva.Value!, cancellationToken);

        return Result<Cliente>.Ok(clienteSalvo with { ContaGrafica = contaSalva.Value });
    }

    public async Task<Result<int>> QuantidadeAtivosAsync(CancellationToken cancellationToken)
    {
        var quantidade = await _clienteRepository.QuantidadeAtivosAsync(cancellationToken);
        return Result<int>.Ok(quantidade);
    }
}