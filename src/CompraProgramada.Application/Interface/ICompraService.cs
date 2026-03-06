namespace CompraProgramada.Application.Interface;

public interface ICompraService
{
    Task ExecutarCompraAsync(CancellationToken cancellationToken);
    Task SeparacaoLoteDeCompra();
}