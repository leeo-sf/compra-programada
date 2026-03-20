using CompraProgramada.Application.Dto;

namespace CompraProgramada.Application.Response;

public record ExecutarCompraResponse(
    DateTime DataExecucao,
    int TotalClientes,
    decimal TotalConsolidado,
    List<OrdemCompraDto> ordensCompra,
    List<DistribuicaoDto> Distribuicoes,
    List<ResiduoCustodiaMasterDto> ResiduosCustMaster,
    int EventosPublicados,
    string Mensagem);