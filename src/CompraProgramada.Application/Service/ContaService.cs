using CompraProgramada.Application.Interface;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;

namespace CompraProgramada.Application.Service;

public class ContaService : IContaService
{
    private readonly IContaRepository _contaRepository;

    public ContaService(IContaRepository contaRepository) => _contaRepository = contaRepository;

    public async Task<Result<ContaGrafica>> GerarContaGraficaAsync(Cliente cliente, CancellationToken cancellationToken)
    {
        var numeroConta = GerarNumeroConta(cliente.Id, ehContaGrafica: true);
        var novaConta = new ContaGrafica(0, numeroConta, DateTime.UtcNow, cliente.Id);
        var contaSalva = await _contaRepository.CreateAsync(novaConta, cancellationToken);
        return Result<ContaGrafica>.Ok(contaSalva);
    }

    public async Task<Result<ContaMaster>> GerarContaMasterAsync(Cliente cliente, CancellationToken cancellationToken)
    {
        var numeroConta = GerarNumeroConta(cliente.Id, ehContaGrafica: false);
        var novaConta = new ContaMaster(0, numeroConta, DateTime.UtcNow);
        var contaSalva = await _contaRepository.CreateAsync(novaConta, cancellationToken);
        return Result<ContaMaster>.Ok(contaSalva);
    }

    private string GerarNumeroConta(int id, bool ehContaGrafica)
    {
        var prefixo = ehContaGrafica ? "FLH" : "MST";
        return $"{prefixo}-{id:D6}";
    }
}