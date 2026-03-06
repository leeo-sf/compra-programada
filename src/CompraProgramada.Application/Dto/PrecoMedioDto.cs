namespace CompraProgramada.Application.Dto;

public class PrecoMedioDto
{
    public required int QuantidadeAtivosAnterior { get; set; }
    public required decimal PrecoMedioAnterior { get; set; }
    public required int QuantidadeNovosAtivos { get; set; }
    public required decimal PrecoFechamentoAtivo { get; set; }
}