using System.Text.Json.Serialization;

namespace CompraProgramada.Application.Dto;

public record HistoricoAporteDto(
    [property: JsonIgnore] int Id,
    DateOnly Data,
    decimal Valor,
    string Parcela);