using CompraProgramada.Domain.Entity;

namespace CompraProgramada.Application.Interface;

public interface ICustodiaService
{
    Task<CustodiaFilhote> GerarCustodiaFilhoteAsync(ContaGrafica conta, CancellationToken cancellationToken);
}