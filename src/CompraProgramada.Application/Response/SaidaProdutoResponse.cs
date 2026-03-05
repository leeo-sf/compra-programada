namespace CompraProgramada.Application.Response;

public class SaidaProdutoResponse
{
    public required int ClienteId { get; set; }
    public required string Nome { get; set; }
    public bool Ativo { get; set; } = false;
    public DateTime DataSaida { get; set; } = DateTime.Now;
    public string Mensagem { get; set; } = "Adesao encerrada. Sua posicao em custodia foi mantida.";
}