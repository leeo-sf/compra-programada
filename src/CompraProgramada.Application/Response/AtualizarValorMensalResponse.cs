namespace CompraProgramada.Application.Response;

public class AtualizarValorMensalResponse
{
    public required int ClienteId { get; set; }
    public required decimal ValorMensalAnterior { get; set; }
    public required decimal ValorMensalNovo { get; set; }
    public DateTime DataAlteracao { get; set; } = DateTime.Now;
    public string Mensagem { get; set; } = "Valor mensal atualizado. O novo valor sera considerado a partir da proxima data de compra.";
}