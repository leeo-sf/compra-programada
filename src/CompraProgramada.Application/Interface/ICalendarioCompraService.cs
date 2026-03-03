namespace CompraProgramada.Application.Interface;

public interface ICalendarioCompraService
{
    bool DeveExecutarCompraHoje();
    DateTime ObterProximaDataCompra();
    DateTime ObterDataReferenciaExecucao(DateTime dataExecutada);
}