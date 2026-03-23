using CompraProgramada.Application.Dto;
using CompraProgramada.Domain.Entity;
using OperationResult;

namespace CompraProgramada.Application.Interface;

public interface ICustodiaMasterService
{
    Task<Result<List<CustodiaMaster>?>> ObterResiduosNaoDistribuidos(CancellationToken cancellationToken);
    Task<Result> AtualizarResiduosAsync(List<CustodiaMaster> grupoAtivos, CancellationToken cancellationToken);
    Task<Result<List<AtivoDto>>> CapturarResiduosNaoDistribuidosAsync(List<Distribuicao> distribuicoes, List<OrdemCompra> ordensCompra, CancellationToken cancellationToken);
}