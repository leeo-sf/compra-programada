namespace CompraProgramada.Domain.Interface;

public interface ICustodiaRepository
{
    Task<T> CreateAsync<T>(T custodia, CancellationToken cancellationToken);
}