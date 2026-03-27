namespace CompraProgramada.Application.Interface;

public interface IFileSystem
{
    bool DiretorioExiste(string caminhoCompleto);
    bool ArquivoExiste(string caminhoCompleto);
    IEnumerable<string> LerLinhas(string caminhoCompleto);
    FileInfo? ObterArquivo(string pasta, string nomeArquivo);
    List<string> ObterArquivos(string pasta, string nomeArquivo);
}