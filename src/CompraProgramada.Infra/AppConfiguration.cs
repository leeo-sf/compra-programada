using CompraProgramada.Application;
using CompraProgramada.Application.Config;
using CompraProgramada.Application.Interface;
using CompraProgramada.Application.Service;
using CompraProgramada.Data;
using CompraProgramada.Data.Repository;
using CompraProgramada.Domain.Interface;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CompraProgramada.Infra;

public static class AppConfiguration
{
    public static void ConfigurarServicosApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigurarMediatR();
        services.ConfigurarFluentValidation();
        services.ConfigurarBancoDeDados(configuration);
        services.AdicionaServicosERepositorios();
        services.ConfigurarRegrasDaAplicacao(configuration);
    }
    public static void ConfigurarServicosWorker(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigurarBancoDeDados(configuration);
        services.AdicionaServicosERepositorios();
        services.ConfigurarRegrasDaAplicacao(configuration);
    }

    private static void ConfigurarBancoDeDados(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetSection("Service:DataBase:ConnectionString").Get<string>();
        services.AddDbContextPool<AppDbContext>(options =>
            options.UseMySql(connectionString,
            ServerVersion.AutoDetect(connectionString),
                opt =>
                {
                    opt.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                    opt.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                }));
    }

    private static void ConfigurarMediatR(this IServiceCollection services)
        => services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Result).Assembly));

    private static void AdicionaServicosERepositorios(this IServiceCollection services)
    {
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IContaMasterRepository, ContaMasterRepository>();
        services.AddScoped<IContaGraficaRepository, ContaGraficaRepository>();
        services.AddScoped<ICotacaoRepository, CotacaoRepository>();
        services.AddScoped<ICustodiaMasterRepository, CustodiaMasterRepository>();
        services.AddScoped<ICestaRecomendadaRepository, CestaRecomendadaRepository>();
        services.AddScoped<IHistoricoExecucaoMotorRepository, HistoricoExecucaoMotorRepository>();

        services.AddScoped<IClienteService, ClienteService>();
        services.AddScoped<IContaGraficaService, ContaGraficaService>();
        services.AddScoped<ICotacaoService, CotacaoService>();
        services.AddScoped<ICustodiaFilhoteService, CustodiaFilhoteService>();
        services.AddScoped<ICustodiaMasterService, CustodiaMasterService>();
        services.AddScoped<ICestaRecomendadaService, CestaRecomendadaService>();
        services.AddScoped<IHistoricoExecucaoMotorService, HistoricoExecucaoMotorService>();
        services.AddScoped<IMotorCompraService, MotorCompraService>();

        services.AddSingleton<ICotahistParserService, CotahistParserService>();
        services.AddSingleton<ICalendarioMotorCompraService, CalendarioMotorCompraService>();
    }

    private static void ConfigurarFluentValidation(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssembly(typeof(Result).Assembly, includeInternalTypes: true);
    }

    private static void ConfigurarRegrasDaAplicacao(this IServiceCollection services, IConfiguration configuration)
        => services.AddSingleton(opt => configuration.GetSection("ApplicationConfig").Get<AppConfig>()!);
}