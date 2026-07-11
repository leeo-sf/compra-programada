namespace CompraProgramada.Domain.Contract.Service;

public interface ICalendarioMotorCompraService
{
    Task<bool> DeveExecutarCompraHoje(CancellationToken cancellationToken);
    DateTime ObterProximaDataCompra();
    DateTime ObterDataReferenciaExecucao(DateTime dataExecutada);
}