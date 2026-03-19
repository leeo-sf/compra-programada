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
    string Mensagem)
{
    private DateOnly dataReferencia;
    private int v1;
    private int v2;
    private List<OrdemCompraDto> ordemCompraDtos;

    public ExecutarCompraResponse(DateOnly dataReferencia, int v1, int v2, List<OrdemCompraDto> ordemCompraDtos)
    {
        this.dataReferencia = dataReferencia;
        this.v1 = v1;
        this.v2 = v2;
        this.ordemCompraDtos = ordemCompraDtos;
    }
}