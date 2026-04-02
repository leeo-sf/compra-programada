using CompraProgramada.Domain.Entity;

namespace CompraProgramada.Domain.Interface;

public interface IClienteRepository
{
    Task<Cliente?> ObterClienteAsync(int id, CancellationToken cancellationToken);
    Task<List<Cliente>> ObterClientesAtivosAsync(CancellationToken cancellationToken);
    Task<int> QuantidadeAtivosAsync(CancellationToken cancellationToken);
    Task<bool> ExisteAsync(string cpf, CancellationToken cancellationToken);
    Task<Cliente> CriarAsync(Cliente cliente, CancellationToken cancellationToken);
    Task<Cliente> AtualizarClienteAsync(Cliente cliente, CancellationToken cancellationToken);
    Task<ContaGrafica> CriarContaAsync(ContaGrafica conta, CancellationToken cancellationToken);
    Task<List<ContaGrafica>> AtualizarContasAsync(List<ContaGrafica> contas, CancellationToken cancellationToken);
}