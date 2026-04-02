using CompraProgramada.Application.Config;
using CompraProgramada.Shared.Dto;
using CompraProgramada.Application.Contract.Service;

namespace CompraProgramada.Application.Service;

public class CotahistParserService : ICotahistParserService
{
    private readonly AppConfig _appConfig;
    private readonly IFileSystem _fileSystem;

    public CotahistParserService(AppConfig config,
        IFileSystem fileSystem)
    {
        _appConfig = config;
        _fileSystem = fileSystem;
    }

    public IEnumerable<CotacaoB3Dto> ParseArquivo()
    {
        var caminhoArquivoUltimoPregao = ObterCaminhoCompletoArquivoCotacoes();

        var cotacoes = new List<CotacaoB3Dto>();

        foreach (var linha in _fileSystem.LerLinhas(caminhoArquivoUltimoPregao))
        {
            // Ignorar header (00) e trailer (99)
            if (linha.Length < 245)
                continue;

            var tipoRegistro = linha.Substring(0, 2);
            if (tipoRegistro != "01")
                continue;

            var tipoMercado = int.Parse(linha.Substring(24, 3).Trim());

            // Filtrar apenas mercado a vista (010) e fracionario (020)
            if (tipoMercado != 10 && tipoMercado != 20)
                continue;

            var cotacao = new CotacaoB3Dto
            {
                DataPregao = DateTime.ParseExact(
                    linha.Substring(2, 8), "yyyyMMdd",
                    System.Globalization.CultureInfo.InvariantCulture),
                CodigoBDI = linha.Substring(10, 2).Trim(),
                Ticker = linha.Substring(12, 12).Trim(),
                TipoMercado = tipoMercado,
                NomeEmpresa = linha.Substring(27, 12).Trim(),
                PrecoAbertura = ParsePreco(linha.Substring(56, 13)),
                PrecoMaximo = ParsePreco(linha.Substring(69, 13)),
                PrecoMinimo = ParsePreco(linha.Substring(82, 13)),
                PrecoMedio = ParsePreco(linha.Substring(95, 13)),
                PrecoFechamento = ParsePreco(linha.Substring(108, 13)),
                QuantidadeNegociada = long.Parse(linha.Substring(152, 18).Trim()),
                VolumeNegociado = ParsePreco(linha.Substring(170, 18))
            };

            cotacoes.Add(cotacao);
        }

        return cotacoes;
    }

    private decimal ParsePreco(string valorBruto)
    {
        if (long.TryParse(valorBruto.Trim(), out var valor))
            return valor / 100m;
        return 0m;
    }

    public string ObterCaminhoCompletoArquivoCotacoes()
    {
        string pastaCotacoes = Path.Combine(AppContext.BaseDirectory, _appConfig.MotorCompraConfig.NomePastaArquivosB3);

        if (!_fileSystem.DiretorioExiste(pastaCotacoes))
            throw new DirectoryNotFoundException($"A pasta {pastaCotacoes} não foi encontrada.");

        var arquivoUltimoPregao = _fileSystem.ObterArquivo(pastaCotacoes, "COTAHIST_D*")
            ?? throw new FileNotFoundException($"Nenhum arquivo encontrado na pasta {pastaCotacoes}");

        return arquivoUltimoPregao.FullName;
    }
}