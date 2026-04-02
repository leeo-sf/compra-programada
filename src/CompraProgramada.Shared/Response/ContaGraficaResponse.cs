namespace CompraProgramada.Shared.Response;

public record ContaGraficaResponse(
    int Id,
    string NumeroConta,
    string Tipo,
    DateTime DataCriacao);