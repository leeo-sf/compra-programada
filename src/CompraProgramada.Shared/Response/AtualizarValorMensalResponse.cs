namespace CompraProgramada.Shared.Response;

public record AtualizarValorMensalResponse(
    int ClienteId,
    decimal ValorMensalAnterior,
    decimal ValorMensalNovo)
{
    public DateTime DataAlteracao { get; } = DateTime.Now;
    public string Mensagem { get; } = "Valor mensal atualizado. O novo valor sera considerado a partir da proxima data de compra.";
}