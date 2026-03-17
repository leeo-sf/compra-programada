namespace CompraProgramada.Application.Response;

public record SaidaProdutoResponse(
    int ClienteId,
    string Nome,
    bool Ativo)
{
    public DateTime DataSaida { get; } = DateTime.Now;
    public string Mensagem { get; } = "Adesao encerrada. Sua posicao em custodia foi mantida.";
}