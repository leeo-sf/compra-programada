namespace CompraProgramada.Application.Contract.Service;

public interface ICalendarioMotorCompraService
{
    bool DeveExecutarCompraHoje();
    DateTime ObterProximaDataCompra();
    DateTime ObterDataReferenciaExecucao(DateTime dataExecutada);
}