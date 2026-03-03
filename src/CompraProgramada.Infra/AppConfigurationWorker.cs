using CompraProgramada.Application.Config;
using CompraProgramada.Application.Interface;
using CompraProgramada.Application.Service;
using CompraProgramada.Data;
using CompraProgramada.Data.Repository;
using CompraProgramada.Domain.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CompraProgramada.Infra;

public static class AppConfigurationWorker
{
    public static void ConfigurarServicosWorker(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigurarBancoDeDados(configuration);
        services.AdicionaServicosERepositorios();
        services.ConfigurarRegrasDaAplicacao(configuration);
    }

    private static void ConfigurarBancoDeDados(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["Service:DataBase:ConnectionString"];
        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
                opt =>
                {
                    opt.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                }));
    }

    private static void AdicionaServicosERepositorios(this IServiceCollection services)
    {
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IContaRepository, ContaRepository>();
        services.AddScoped<ICustodiaRepository, CustodiaRepository>();
        services.AddScoped<ICestaRecomendadaRepository, CestaRecomendadaRepository>();
        services.AddScoped<IHistoricoExecucaoMotorRepository, HistoricoExecucaoMotorRepository>();

        services.AddScoped<IClienteService, ClienteService>();
        services.AddScoped<IContaService, ContaService>();
        services.AddScoped<ICustodiaService, CustodiaService>();
        services.AddScoped<ICestaRecomendadaService, CestaRecomendadaService>();
        services.AddScoped<IHistoricoExecucaoMotorService, HistoricoExecucaoMotorService>();

        services.AddSingleton<ICotahistParser, CotahistParser>();
        services.AddSingleton<ICalendarioCompraService, CalendarioCompraService>();
    }

    private static void ConfigurarRegrasDaAplicacao(this IServiceCollection services, IConfiguration configuration)
        => services.AddSingleton(opt => configuration.GetSection("ApplicationConfig").Get<AppConfig>()!);
}