using System.Text.Json.Serialization;

namespace CompraProgramada.Application.Dto;

public record OrdemCompraDto(
    [property: JsonIgnore] int Id,
    string Ticker,
    int QuantidadeTotal,
    List<OrdemCompraDetalheDto> Detalhes,
    decimal PrecoUnitario)
{
    public decimal ValorTotal => QuantidadeTotal * PrecoUnitario;
}