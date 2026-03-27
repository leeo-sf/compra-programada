using CompraProgramada.Application.Interface;

namespace CompraProgramada.Application.Service;

public class FileSystem : IFileSystem
{
    public bool ArquivoExiste(string caminhoCompleto)
        => File.Exists(caminhoCompleto);

    public bool DiretorioExiste(string caminhoCompleto)
        => Directory.Exists(caminhoCompleto);

    public IEnumerable<string> LerLinhas(string caminhoCompleto)
        => File.ReadLines(caminhoCompleto);

    public FileInfo? ObterArquivo(string pasta, string nomeArquivo)
        => Directory.GetFiles(pasta, nomeArquivo)
            .Select(caminho => new FileInfo(caminho))
            .OrderByDescending(f => f.Name)
            .FirstOrDefault();

    public List<string> ObterArquivos(string pasta, string nomeArquivo)
        => Directory.GetFiles(pasta, nomeArquivo)
            .OrderByDescending(f => f)
            .ToList();
}