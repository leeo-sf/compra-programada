using CompraProgramada.Shared.Dto;

namespace CompraProgramada.Shared.Response;

public record CriarCestaRecomendadaResponse(
    int CestaId,
    string Nome,
    bool Ativa,
    DateTime DataCriacao,
    List<ComposicaoCestaDto> Itens,
    CestaDesativadaDto? CestaAnteriorDesativada,
    List<string>? AtivosRemovidos,
    List<string>? AtivosAdicionados,
    bool RebalanceamentoDisparado = false,
    string Mensagem = "Primeira cesta cadastrada com sucesso.");