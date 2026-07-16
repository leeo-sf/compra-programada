using CompraProgramada.Data;
using CompraProgramada.Data.Repository;
using CompraProgramada.Domain;
using CompraProgramada.Domain.Contract.Handler;
using CompraProgramada.Domain.Contract.Repository;
using CompraProgramada.Domain.Contract.Service;
using CompraProgramada.Domain.Handler.Worker;
using CompraProgramada.Domain.Mapper;
using CompraProgramada.Domain.Service;
using CompraProgramada.Infra.Converter;
using CompraProgramada.Shared;
using CompraProgramada.Shared.Config;
using CompraProgramada.Shared.Request;
using CompraProgramada.Shared.Response;
using Confluent.Kafka;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OperationResult;
using System.Text.Json.Serialization;

namespace CompraProgramada.Infra;

public static class AppConfiguration
{
    private static readonly Type[] _handlerTypes =
    [
        typeof(IApiRequestHandler),
        typeof(IWorkerMotorCompraRequestHandler)
    ];

    public static void ConfigurarServicosApi(this IServiceCollection services, IConfiguration configuration, ServerVersion? serverVersion = null)
    {
        services.AddMediatR(x =>
            x.RegisterServicesFromAssembly(typeof(DomainExceptionHandler).Assembly))
            .ConfigurarHandlers<IApiRequestHandler>();

        // Adiciona o handler do worker na API devido a disponibilidade de um endpoint (com finalidade para testes) para executar o motor de compra manualmente.
        // Essa configuração está sendo feita manualmente devido o handler do worker ser removido na configuração de handlers no método ConfigurarHandlers.
        services.AddTransient<IRequestHandler<ExecutarMotorCompraRequest, Result<ExecutarMotorCompraResponse>>, MotorCompraHandler>();

        services.ConfigureHttpOptions();
        services.ConfigurarExceptionHandler();
        services.ConfigurarFluentValidation();
        services.ConfigurarBancoDeDados(configuration, serverVersion);
        services.AdicionaServicosERepositorios();
        services.ConfigurarRegrasDaAplicacao(configuration);
        services.ConfigurarKafka(configuration);
        services.ConfigurarMappers();
    }

    public static void ConfigurarServicosWorker(this IServiceCollection services, IConfiguration configuration, ServerVersion? serverVersion = null)
    {
        services.AddMediatR(x =>
            x.RegisterServicesFromAssembly(typeof(DomainExceptionHandler).Assembly))
            .ConfigurarHandlers<IWorkerMotorCompraRequestHandler>();

        services.ConfigurarBancoDeDados(configuration, serverVersion);
        services.AdicionaServicosERepositorios();
        services.ConfigurarRegrasDaAplicacao(configuration);
        services.ConfigurarKafka(configuration);
        services.ConfigurarMappers();
    }

    internal static void ConfigurarBancoDeDados(this IServiceCollection services, IConfiguration configuration, ServerVersion? serverVersion)
    {
        var connectionString = configuration.GetSection("Service:DataBase:ConnectionString").Get<string>();

        serverVersion ??= new MySqlServerVersion(new Version(8, 4, 0));

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

    internal static void AdicionaServicosERepositorios(this IServiceCollection services)
    {
        services.AddScoped<ICestaRecomendadaRepository, CestaRecomendadaRepository>();
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IContaMasterRepository, ContaMasterRepository>();
        services.AddScoped<ICotacaoRepository, CotacaoRepository>();
        services.AddScoped<ICustodiaMasterRepository, CustodiaMasterRepository>();
        services.AddScoped<IHistoricoExecucaoMotorRepository, HistoricoExecucaoMotorRepository>();
        services.AddScoped<IOrdemCompraRepository, OrdemCompraRepository>();

        services.AddScoped<ICotacaoService, CotacaoService>();
        services.AddScoped<IOrdemCompraService, OrdemCompraService>();
        services.AddScoped<ICalendarioMotorCompraService, CalendarioMotorCompraService>();

        services.AddSingleton<ICotahistParserService, CotahistParserService>();
        services.AddSingleton<IFileSystem, FileSystem>();
        services.AddSingleton<IImpostoRendaService, ImpostoRendaService>();
        services.AddSingleton<IDateTimeProvaider, DateTimeProvaider>();
    }

    internal static void ConfigurarFluentValidation(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssembly(typeof(SharedAssembly).Assembly, includeInternalTypes: true);
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

    private static void ConfigureHttpOptions(this IServiceCollection services)
        => services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new UtcDateTimeConverter());
            options.SerializerOptions.Converters.Add(new DecimalTwoDecimalPlacesConverter());
            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });

    private static void ConfigurarHandlers<THandler>(this IServiceCollection services)
        where THandler : IBaseRequestHandler
    {
        var handlerType = typeof(THandler);

        var activeHandlerInterface = _handlerTypes
            .First(x => x.IsAssignableFrom(handlerType));

        var descriptorsToRemove = services
            .Where(d => d.ImplementationType is not null
               && typeof(IBaseRequestHandler).IsAssignableFrom(d.ImplementationType)
               && !activeHandlerInterface.IsAssignableFrom(d.ImplementationType))
            .ToList();

        foreach (var descriptor in descriptorsToRemove)
            services.Remove(descriptor);
    }
}