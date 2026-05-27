using CompraProgramada.Shared.Dto;

namespace CompraProgramada.Domain.Contract.Service;

public interface ICotahistParserService
{
    IEnumerable<CotacaoB3Dto> ParseArquivo();
}