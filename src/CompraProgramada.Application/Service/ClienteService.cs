using CompraProgramada.Application.Exceptions;
using CompraProgramada.Application.Interface;
using CompraProgramada.Application.Request;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;

namespace CompraProgramada.Application.Service;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IContaGraficaService _contaService;
    private readonly ICestaRecomendadaService _cestaRecomendadaService;

    public ClienteService(IClienteRepository clienteRepository,
        IContaGraficaService contaService,
        ICestaRecomendadaService cestaRecomendadaService)
    {
        _clienteRepository = clienteRepository;
        _contaService = contaService;
        _cestaRecomendadaService = cestaRecomendadaService;
    }

    public async Task<Result<List<Cliente>>> ObtemClientesAtivoAsync(CancellationToken cancellationToken)
    {
        var clientes = await _clienteRepository.ObterClientesAtivosAsync(cancellationToken);

        return Result<List<Cliente>>.Ok(clientes);
    }

    public async Task<Result<Cliente>> AdesaoAsync(AdesaoRequest request, CancellationToken cancellationToken)
    {
        if (request.ValorMensal < 100)
            return Result<Cliente>.Fail(new ErroMapeadoException("O valor mensal minimo e de R$ 100,00.", "VALOR_MENSAL_INVALIDO"));

        var clienteExistente = await _clienteRepository.ExisteAsync(request.Cpf, cancellationToken);

        if (clienteExistente)
            return Result<Cliente>.Fail(new ErroMapeadoException("CPF ja cadastrado no sistema.", "CLIENTE_CPF_DUPLICADO"));

        var cestaVigente = await _cestaRecomendadaService.ObterCestaAtivaAsync(cancellationToken);

        if (!cestaVigente.IsSuccess || cestaVigente.Value is null)
            return Result<Cliente>.Fail(new ErroMapeadoException("Cesta Top Five ainda não foi cadastrada.", "CESTA_TOP_FIVE_NAO_ENCONTRADA"));

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

        return Result<Cliente>.Ok(clienteSalvo);
    }

    public async Task<Result<int>> QuantidadeAtivosAsync(CancellationToken cancellationToken)
    {
        var quantidade = await _clienteRepository.QuantidadeAtivosAsync(cancellationToken);
        return Result<int>.Ok(quantidade);
    }

    public async Task<Result> RealizaDistribuicaoAsync(List<ContaGrafica> contas, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}