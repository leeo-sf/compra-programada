using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Interface;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;
using OperationResult;

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

    public async Task<Result<ContaGraficaDto>> GerarContaGraficaAsync(int clienteId, CancellationToken cancellationToken)
    {
        var cestaVigente = await _cestaRecomendadaService.ObterCestaAtivaAsync(cancellationToken);

        var custodiaConta = cestaVigente.Value!.Itens
            .Select(x => new CustodiaFilhote(0, 0, x.Ticker)).ToList();

        var numeroConta = GerarNumeroConta(clienteId, ehContaGrafica: true);
        var novaConta = new ContaGrafica(0, numeroConta, DateTime.Now, clienteId) { CustodiaFilhotes = custodiaConta };

        var contaSalva = await _contaGraficaRepository.CriarAsync(novaConta, cancellationToken);

        return new ContaGraficaDto(
            contaSalva.Id,
            contaSalva.NumeroConta,
            contaSalva.DataCriacao,
            contaSalva.ClienteId,
            contaSalva.Tipo,
            null,
            contaSalva.CustodiaFilhotes.Select(cf => new CustodiaFilhoteDto(
                cf.Id,
                cf.ContaGraficaId,
                cf.Ticker ?? string.Empty,
                cf.PrecoMedio,
                cf.Quantidade
            )).ToList()
        );
    }

    public async Task<Result> RegistrarComprasAsync(List<HistoricoCompraDto> compras, CancellationToken cancellationToken)
    {
        if (!compras.Any())
            return new ApplicationException("Nenhuma compra informada para registro.");

        var comprarAhRegistrar = compras.Select(hc => HistoricoCompra.RegistrarHistorico(hc.ContaGraficaId, hc.Ticker, hc.Quantidade, hc.PrecoExecutado, hc.PrecoMedio, hc.ValorAporte, hc.Data)).ToList();

        await _contaGraficaRepository.RegistrarHistoricoCompraAysnc(comprarAhRegistrar, cancellationToken);

        return Result.Success();
    }

    private string GerarNumeroConta(int id, bool ehContaGrafica)
    {
        var prefixo = ehContaGrafica ? "FLH" : "MST";
        return $"{prefixo}-{id:D6}";
    }
}