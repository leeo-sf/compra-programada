using CompraProgramada.Application.Dto;

namespace CompraProgramada.Application.Interface;

public interface ICotahistParserService
{
    IEnumerable<CotacaoB3Dto> ParseArquivo();
}