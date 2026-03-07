using CompraProgramada.Application.Dto;
using OperationResult;

namespace CompraProgramada.Application.Interface;

public interface IDistribuicaoService
{
    Task<(List<GrupoAtivoCompraDto>, List<FechamentoAtivoB3Dto>)> RealizaDistribuicaoGrupoAtivo(List<ClienteDto> clientesAtivos, decimal totalConsolidado, DateTime dataExeucao, CancellationToken cancellationToken);
    Task<Result<List<DistribuicaoDto>>> DistribuirCustodiasPorAtivo(List<ClienteDto> clientes, List<GrupoAtivoCompraDto> grupoAtivoCompra, decimal totalConsolidado, DateTime dataExeucao, CancellationToken cancellationToken);
    Task<Result> SalvarRegistroDistribuicoes(List<DistribuicaoDto> ditribuicoes, List<OrdemCompraDto> ordensCompraAtivos, CancellationToken cancellationToken);
}