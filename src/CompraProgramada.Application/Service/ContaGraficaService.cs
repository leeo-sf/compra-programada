using CompraProgramada.Application.Interface;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;

namespace CompraProgramada.Application.Service;

public class ContaGraficaService : IContaGraficaService
{
    private readonly ICestaRecomendadaService _cestaRecomendadaService;
    private readonly IContaGraficaRepository _contaGraficaRepository;

    public ContaGraficaService(ICestaRecomendadaService cestaRecomendadaService,
        IContaGraficaRepository contaGraficaRepository)
    {
        _cestaRecomendadaService = cestaRecomendadaService;
        _contaGraficaRepository = contaGraficaRepository;
    }

    public async Task<Result<ContaGrafica>> GerarContaGraficaAsync(Cliente cliente, CancellationToken cancellationToken)
    {
        var cestaVigente = await _cestaRecomendadaService.ObterCestaAtivaAsync(cancellationToken);

        var custodiaConta = cestaVigente.Value!.ComposicaoCesta
            .Select(x => new CustodiaFilhote(0, 0, x.Ticker)).ToList();

        var numeroConta = GerarNumeroConta(cliente.Id, ehContaGrafica: true);
        var novaConta = new ContaGrafica(0, numeroConta, DateTime.UtcNow, cliente.Id) { CustodiaFilhotes = custodiaConta };

        var contaSalva = await _contaGraficaRepository.CriarAsync(novaConta, cancellationToken);

        return Result<ContaGrafica>.Ok(contaSalva);
    }

    public async Task<Result> AlterarCustodiasAsync(List<ContaGrafica> contas, CancellationToken cancellationToken)
    {
        if (!contas.Any())
            return Result.Fail(new ApplicationException("Nenhuma conta foi informada."));

        await _contaGraficaRepository.AtualizarCustodiasAysnc(contas, cancellationToken);

        return Result.Ok();
    }

    private string GerarNumeroConta(int id, bool ehContaGrafica)
    {
        var prefixo = ehContaGrafica ? "FLH" : "MST";
        return $"{prefixo}-{id:D6}";
    }
}