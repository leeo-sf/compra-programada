using System.Text.Json.Serialization;

namespace CompraProgramada.Application.Dto;

public record EvolucaoCarteiraDto(
    [property: JsonIgnore] int Id,
    DateOnly Data,
    decimal ValorCarteira,
    decimal ValorInvestido,
    decimal Rentabilidade);