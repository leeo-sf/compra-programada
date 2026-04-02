using CompraProgramada.Application.Config;
using CompraProgramada.Application.Contract.Service;
using CompraProgramada.Application.Mapper;
using CompraProgramada.Application.Service;
using CompraProgramada.Data;
using CompraProgramada.Data.Repository;
using CompraProgramada.Domain.Interface;
using CompraProgramada.Infra.Converter;
using CompraProgramada.Infra.Middleware;
using Confluent.Kafka;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CompraProgramada.Infra;

public static class AppConfiguration
{
    public static void ConfigurarServicosApi(this IServiceCollection services, IConfiguration configuration, ServerVersion? serverVersion = null)
    {
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new UtcDateTimeConverter());
            });

        services.ConfigurarExceptionHandler();
        services.ConfigurarMediatR();
        services.ConfigurarFluentValidation();
        services.ConfigurarBancoDeDados(configuration, serverVersion);
        services.AdicionaServicosERepositorios();
        services.ConfigurarRegrasDaAplicacao(configuration);
        services.ConfigurarKafka(configuration);
        services.ConfigurarMappers();
    }

    public static void ConfigurarServicosWorker(this IServiceCollection services, IConfiguration configuration, ServerVersion? serverVersion = null)
    {
        services.ConfigurarBancoDeDados(configuration, serverVersion);
        services.AdicionaServicosERepositorios();
        services.ConfigurarRegrasDaAplicacao(configuration);
        services.ConfigurarKafka(configuration);
    }

    internal static void ConfigurarBancoDeDados(this IServiceCollection services, IConfiguration configuration, ServerVersion? serverVersion)
    {
        var connectionString = configuration.GetSection("Service:DataBase:ConnectionString").Get<string>();

        serverVersion ??= ServerVersion.AutoDetect(connectionString);

        services.AddDbContextPool<AppDbContext>(options =>
            options.UseMySql(connectionString,
            serverVersion,
                opt =>
                {
                    opt.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                    opt.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                }));
    }

    internal static void ConfigurarMediatR(this IServiceCollection services)
        => services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AppConfig).Assembly));

    internal static void AdicionaServicosERepositorios(this IServiceCollection services)
    {
        services.AddScoped<ICestaRecomendadaRepository, CestaRecomendadaRepository>();
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IContaMasterRepository, ContaMasterRepository>();
        services.AddScoped<ICotacaoRepository, CotacaoRepository>();
        services.AddScoped<ICustodiaFilhoteRepository, CustodiaFilhoteRepository>();
        services.AddScoped<ICustodiaMasterRepository, CustodiaMasterRepository>();
        services.AddScoped<IHistoricoExecucaoMotorRepository, HistoricoExecucaoMotorRepository>();
        services.AddScoped<IOrdemCompraRepository, OrdemCompraRepository>();

        services.AddScoped<ICestaRecomendadaService, CestaRecomendadaService>();
        services.AddScoped<IClienteService, ClienteService>();
        services.AddScoped<ICotacaoService, CotacaoService>();
        services.AddScoped<ICustodiaMasterService, CustodiaMasterService>();
        services.AddScoped<IDistribuicaoService, DistribuicaoService>();
        services.AddScoped<IHistoricoExecucaoMotorService, HistoricoExecucaoMotorService>();
        services.AddScoped<ICompraService, CompraService>();
        services.AddScoped<IOrdemCompraService, OrdemCompraService>();

        services.AddSingleton<ICotahistParserService, CotahistParserService>();
        services.AddSingleton<ICalendarioMotorCompraService, CalendarioMotorCompraService>();
        services.AddSingleton<IFileSystem, FileSystem>();
        services.AddSingleton<IImpostoRendaService, ImpostoRendaService>();
        services.AddSingleton<IDateTimeProvaider, DateTimeProvaider>();
    }

    internal static void ConfigurarFluentValidation(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssembly(typeof(AppConfig).Assembly, includeInternalTypes: true);
    }

    internal static void ConfigurarRegrasDaAplicacao(this IServiceCollection services, IConfiguration configuration)
        => services.AddSingleton(opt => configuration.GetSection("ApplicationConfig").Get<AppConfig>()!);

    internal static void ConfigurarKafka(this IServiceCollection services, IConfiguration configuration)
    {
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = configuration.GetSection("Service:Kafka:Server").Get<string>(),
            Acks = Acks.All,
            MessageSendMaxRetries = configuration.GetSection("Service:Kafka:SendMaxRetries").Get<int>()
        };

        services.AddSingleton(producerConfig);
        services.AddSingleton<IKafkaProducer, KafkaProducer>();
    }

    internal static void ConfigurarExceptionHandler(this IServiceCollection services)
    {
        services.AddExceptionHandler<DomainExceptionHandler>();
        services.AddProblemDetails();
    }

    private static void ConfigurarMappers(this IServiceCollection services)
    {
        services.AddSingleton<CestaRecomendadaMapper>();
        services.AddSingleton<ClienteMapper>();
        services.AddSingleton<ContaMapper>();
        services.AddSingleton<CustodiaFilhoteMapper>();
        services.AddSingleton<DistribuicaoMapper>();
        services.AddSingleton<HistoricoCompraMapper>();
        services.AddSingleton<OrdemCompraMapper>();
    }
}