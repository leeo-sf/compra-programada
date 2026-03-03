namespace CompraProgramada.Application.Dto;

public class CotacaoDto
{
    public DateTime DataPregao { get; set; }
    public List<ComposicaoCotacaoDto> Itens { get; set; } = default!;
}