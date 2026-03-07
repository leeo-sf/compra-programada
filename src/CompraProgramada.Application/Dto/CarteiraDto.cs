namespace CompraProgramada.Application.Dto;

public record CarteiraDto(
    ResumoCarteiraDto Resumo, List<DetalheAtivoCarteiraDto> Ativos);