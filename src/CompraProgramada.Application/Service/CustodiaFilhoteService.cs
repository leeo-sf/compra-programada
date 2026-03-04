using CompraProgramada.Application.Interface;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;

namespace CompraProgramada.Application.Service;

public class CustodiaFilhoteService : ICustodiaFilhoteService
{
    private readonly ICustodiaMasterRepository _custodiaRepository;

    public CustodiaFilhoteService(ICustodiaMasterRepository custodiaRepository) => _custodiaRepository = custodiaRepository;

    public async Task<CustodiaFilhote> AtualizarCustodiaFilhoteAsync(ContaGrafica conta, CancellationToken cancellationToken)
    {
        var custodia = new CustodiaFilhote(0, conta.Id, default, default);
        //var custodiaSalva = await _custodiaRepository.CreateAsync(custodia, cancellationToken);
        return custodia;
    }
}