using System.Text.Json.Serialization;

namespace CompraProgramada.Application.Dto;

public record DistribuicaoDto(
    [property: JsonIgnore] int Id,
    [property: JsonIgnore] string Cpf,
    [property: JsonIgnore] int OrdemCompraId,
    [property: JsonIgnore] int ContaGraficaId,
    [property: JsonIgnore] string Ticker,
    [property: JsonIgnore] int QuantidadeAlocada,
    [property: JsonIgnore] decimal ValorOperacao,
    [property: JsonIgnore] ContaGraficaDto ContaGrafica,
    [property: JsonIgnore] DateTime Data,
    int ClienteId,
    string Nome,
    decimal ValorAporte,
    List<AtivoDto> Ativos);