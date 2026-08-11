using CompraProgramada.Shared.Dto;

namespace CompraProgramada.Shared.Response;

public record ConsultarCustodiaMasterResponse(ContaMasterDto contaMaster, IEnumerable<CustodiaMasterDto> custodia)
{
    public decimal ValorTotalInvestido => custodia.Sum(c => c.ValorAtual);
}