using CompraProgramada.Domain.Entity;

namespace CompraProgramada.Application.Interface;

public interface ICustodiaFilhoteService
{
    Task<CustodiaFilhote> AtualizarCustodiaFilhoteAsync(ContaGrafica conta, CancellationToken cancellationToken);
}