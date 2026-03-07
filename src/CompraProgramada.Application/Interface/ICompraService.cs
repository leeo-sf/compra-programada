namespace CompraProgramada.Application.Interface;

public interface ICompraService
{
    Task ExecutarCompraAsync(DateTime? data, CancellationToken cancellationToken);
    Task SeparacaoLoteDeCompra();
}