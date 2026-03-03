namespace CompraProgramada.Domain.Interface;

public interface IContaRepository
{
    Task<T> CreateAsync<T>(T conta, CancellationToken cancellationToken);
}