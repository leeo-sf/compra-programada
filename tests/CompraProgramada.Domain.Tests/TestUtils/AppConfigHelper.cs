using CompraProgramada.Shared.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace CompraProgramada.Domain.Tests.TestUtils;

public class AppConfigHelper
{
    public static AppConfig GetAppConfig()
    {
        var settings = new AppConfig
        {
            MotorCompraConfig = new MotorCompraConfig
            {
                DiasDeCompra = new[] { 5, 15, 25 },
                TempoEmHoraAhCadaExecucao = 10,
                NomePastaArquivosB3 = "cotacoes"
            }
        };

        return settings;
    }
}