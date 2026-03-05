using CompraProgramada.Application.Dto;

namespace CompraProgramada.Application.Response;

public class CarteiraCustodiaResponse(ClienteDto cliente, ResumoCarteiraDto resumo)
{
    public int ClienteId { get; } = cliente.ClienteId;
    public string Nome { get; } = cliente.Nome;
    public string ContaGrafica = cliente.ContaGrafica.NumeroConta;
    public DateTime DataConsulta { get; } = DateTime.Now;
    public ResumoCarteiraDto Resumo { get; } = resumo;
    public List<DetalheAtivosCarteiraDto> Ativos { get; }
}