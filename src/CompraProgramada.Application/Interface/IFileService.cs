namespace CompraProgramada.Application.Interface;

public interface IFileService
{
    string ObterCaminhoCompletoArquivoCotacoes();
    bool ArquivoExiste(string caminhoCompleto);
}