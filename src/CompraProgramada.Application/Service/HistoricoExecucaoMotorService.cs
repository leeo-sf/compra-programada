using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Interface;
using CompraProgramada.Domain.Interface;

namespace CompraProgramada.Application.Service;

public class HistoricoExecucaoMotorService : IHistoricoExecucaoMotorService
{
    private readonly IHistoricoExecucaoMotorRepository _historicoExecucaoRepository;
    private readonly ICalendarioCompraService _calendarioCompraService;

    public HistoricoExecucaoMotorService(IHistoricoExecucaoMotorRepository historicoExecucaoRepository,
        ICalendarioCompraService calendarioCompraService)
    {
        _historicoExecucaoRepository = historicoExecucaoRepository;
        _calendarioCompraService = calendarioCompraService;
    }

    public async Task<bool> ExecutarCompraHojeAsync(CancellationToken cancellationToken)
    {
        var deveExecutarCompraHoje = _calendarioCompraService.DeveExecutarCompraHoje();

        if (deveExecutarCompraHoje)
        {
            var jaFoiExecutada = await _historicoExecucaoRepository.ObtemExecucaoRealizadaAsync(DateTime.Now, cancellationToken);

            if (jaFoiExecutada is not null)
                return false;

            return true;
        }

        return false;
    }

    public async Task SalvarExecucaoAsync(CompraExecucaoDto execucaoCompra, CancellationToken cancellationToken)
        => await _historicoExecucaoRepository.CriarHistoricoExecucaoAsync(
            new (0, execucaoCompra.DataReferencia, execucaoCompra.DataExecucao, execucaoCompra.Executado),
            cancellationToken);
}