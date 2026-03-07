namespace CompraProgramada.Application.Dto;

public class OrdemCompraDto(
    int id,
    string ticker,
    int quantidadeTotal,
    List<DetalheOrdemCompraDto> detalhes,
    decimal precoUnitario)
{
    public int Id { get; private set; } = id;
    public string Ticker { get; private set; } = ticker;
    public int QuantidadeTotal { get; private set; } = quantidadeTotal;
    public List<DetalheOrdemCompraDto> Detalhes { get; private set; } = detalhes;
    public decimal PrecoUnitario { get; private set; } = precoUnitario;
    public decimal ValorTotal => QuantidadeTotal * PrecoUnitario;
}