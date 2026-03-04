namespace CompraProgramada.Application.Interface;

public interface ICalendarioMotorCompraService
{
    bool DeveExecutarCompraHoje();
    DateTime ObterProximaDataCompra();
    DateTime ObterDataReferenciaExecucao(DateTime dataExecutada);
}