using CompraProgramada.Shared.Dto;

namespace CompraProgramada.Shared.Response;

public record ExecutarCompraResponse(
    DateTime DataExecucao,
    int TotalClientes,
    decimal TotalConsolidado,
    List<OrdemCompraDto> ordensCompra,
    List<DistribuicaoDto> Distribuicoes,
    List<AtivoQuantidadeDto> ResiduosCustMaster,
    int EventosPublicados,
    string Mensagem);