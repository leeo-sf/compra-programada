using CompraProgramada.Application.Config;
using CompraProgramada.Application.Interface;

namespace CompraProgramada.Application.Service;

public class FileService : IFileService
{
    private readonly AppConfig _appConfig;

    public FileService(AppConfig appConfig) => _appConfig = appConfig;

    public string ObterCaminhoCompletoArquivoCotacoes()
    {
        string pastaCotacoes = Path.Combine(AppContext.BaseDirectory, _appConfig.MotorCompraConfig.NomePastaArquivosB3);

        if (!ArquivoExiste(pastaCotacoes))
            throw new DirectoryNotFoundException($"A pasta {pastaCotacoes} não foi encontrada.");

        var arquivoUltimoPregao = Directory.GetFiles(pastaCotacoes, "COTAHIST_D*")
            .Select(caminho => new FileInfo(caminho))
            .OrderByDescending(f => f.Name)
            .FirstOrDefault();

        if (!File.Exists(arquivoUltimoPregao!.FullName))
            throw new FileNotFoundException($"Nenhum arquivo com a data pregão mais recente foi encontrado.");

        return arquivoUltimoPregao.FullName;
    }

    public bool ArquivoExiste(string caminhoCompleto)
        => Directory.Exists(caminhoCompleto);
}