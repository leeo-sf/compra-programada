using CompraProgramada.Application.Dto;

namespace CompraProgramada.Application.Interface;

public interface ICotahistParser
{
    IEnumerable<CotacaoB3Dto> ParseArquivo(string caminhoArquivo);
    CotacaoB3Dto? ObterCotacaoFechamento(string pastaCotacoes, string ticker);
}