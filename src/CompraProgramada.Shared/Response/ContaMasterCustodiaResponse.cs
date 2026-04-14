using CompraProgramada.Shared.Dto;

namespace CompraProgramada.Shared.Response;

public record ContaMasterCustodiaResponse(
    ContaMasterDto ContaMaster,
    List<CustodiaMasterDto> Custodia)
{
    public decimal ValorTotalResiduo => Custodia.Sum(x => x.ValorAtual);
}