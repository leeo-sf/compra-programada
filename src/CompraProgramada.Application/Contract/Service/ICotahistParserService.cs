using CompraProgramada.Shared.Dto;

namespace CompraProgramada.Application.Contract.Service;

public interface ICotahistParserService
{
    IEnumerable<CotacaoB3Dto> ParseArquivo();
}