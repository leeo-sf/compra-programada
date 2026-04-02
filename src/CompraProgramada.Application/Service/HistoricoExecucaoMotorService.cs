using CompraProgramada.Application.Contract.Service;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;

namespace CompraProgramada.Application.Service;

public class HistoricoExecucaoMotorService : IHistoricoExecucaoMotorService
{
    private readonly IHistoricoExecucaoMotorRepository _historicoExecucaoRepository;
    private readonly ICalendarioMotorCompraService _calendarioCompraService;

    public HistoricoExecucaoMotorService(IHistoricoExecucaoMotorRepository historicoExecucaoRepository,
        ICalendarioMotorCompraService calendarioCompraService)
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

    public async Task SalvarExecucaoAsync(DateTime dataReferencia, DateTime dataExecucao, CancellationToken cancellationToken)
        => await _historicoExecucaoRepository.CriarHistoricoExecucaoAsync(
            HistoricoExecucaoMotor.CriarRegistroHistorico(dataReferencia, dataExecucao),
            cancellationToken);
}