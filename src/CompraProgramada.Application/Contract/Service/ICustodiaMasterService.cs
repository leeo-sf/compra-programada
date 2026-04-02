using CompraProgramada.Domain.Entity;
using CompraProgramada.Shared.Dto;
using OperationResult;

namespace CompraProgramada.Application.Contract.Service;

public interface ICustodiaMasterService
{
    Task<Result<List<CustodiaMaster>?>> ObterResiduosNaoDistribuidos(CancellationToken cancellationToken);
    Task<Result> AtualizarResiduosAsync(List<CustodiaMaster> grupoAtivos, CancellationToken cancellationToken);
    Task<Result<List<AtivoQuantidadeDto>>> CapturarResiduosNaoDistribuidosAsync(List<Distribuicao> distribuicoes, List<OrdemCompra> ordensCompra, CancellationToken cancellationToken);
}