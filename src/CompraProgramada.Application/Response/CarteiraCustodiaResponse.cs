using CompraProgramada.Application.Dto;

namespace CompraProgramada.Application.Response;

public record CarteiraCustodiaResponse(
    int ClienteId,
    string Nome,
    string ContaGrafica,
    ResumoCarteiraDto resumo,
    List<DetalheAtivoCarteiraDto> Ativos)
{
    public DateTime DataConsulta => DateTime.Now;
}