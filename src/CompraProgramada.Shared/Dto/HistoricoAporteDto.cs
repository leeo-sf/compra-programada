namespace CompraProgramada.Shared.Dto;

public class HistoricoAporteDto
{
    public required DateOnly Data { get; init; }
    public required decimal Valor { get; init; }
    public required string Parcela { get; init; }
}