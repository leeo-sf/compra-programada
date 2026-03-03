using CompraProgramada.Application.Interface;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;

namespace CompraProgramada.Application.Service;

public class CustodiaService : ICustodiaService
{
    private readonly ICustodiaRepository _custodiaRepository;

    public CustodiaService(ICustodiaRepository custodiaRepository) => _custodiaRepository = custodiaRepository;

    public async Task<CustodiaFilhote> GerarCustodiaFilhoteAsync(ContaGrafica conta, CancellationToken cancellationToken)
    {
        var custodia = new CustodiaFilhote(0, conta.Id, default, default);
        var custodiaSalva = await _custodiaRepository.CreateAsync(custodia, cancellationToken);
        return custodiaSalva;
    }
}