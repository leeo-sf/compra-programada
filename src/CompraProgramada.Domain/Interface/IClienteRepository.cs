using CompraProgramada.Domain.Entity;

namespace CompraProgramada.Domain.Interface;

public interface IClienteRepository
{
    Task<List<Cliente>> ObterClientesAtivosAsync(CancellationToken cancellationToken);
    Task<int> QuantidadeAtivosAsync(CancellationToken cancellationToken);
    Task<bool> ExisteAsync(string cpf, CancellationToken cancellationToken);
    Task<Cliente> CriarAsync(Cliente cliente, CancellationToken cancellationToken);
}