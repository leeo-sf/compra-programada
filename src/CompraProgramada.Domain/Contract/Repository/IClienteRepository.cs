using CompraProgramada.Domain.Entity;

namespace CompraProgramada.Domain.Contract.Repository;

public interface IClienteRepository
{
    Task<Cliente?> ObterAsync(int id, CancellationToken cancellationToken);
    Task<List<Cliente>> ObterClientesAtivosAsync(CancellationToken cancellationToken);
    Task<long> QuantidadeAtivosAsync(CancellationToken cancellationToken);
    Task<bool> CpfExistenteAsync(string cpf, CancellationToken cancellationToken);
    Task<Cliente> CriarAsync(Cliente cliente, CancellationToken cancellationToken);
    Task<Cliente> AtualizarAsync(Cliente cliente, CancellationToken cancellationToken);
    Task<ContaGrafica> CriarContaAsync(ContaGrafica conta, CancellationToken cancellationToken);
    Task<List<ContaGrafica>> AtualizarContasAsync(List<ContaGrafica> contas, CancellationToken cancellationToken);
}