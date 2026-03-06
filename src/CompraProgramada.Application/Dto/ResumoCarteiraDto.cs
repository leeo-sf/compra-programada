namespace CompraProgramada.Application.Dto;

public record ResumoCarteiraDto(
    decimal ValorTotalInvestido,
    decimal ValorAtualCarteira,
    decimal PlTotal,
    decimal RentabilidadePercentual);