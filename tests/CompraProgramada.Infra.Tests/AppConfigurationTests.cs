using CompraProgramada.Application.Config;
using CompraProgramada.Shared.Dto;
using CompraProgramada.Shared.Request;
using CompraProgramada.Application.Service;
using CompraProgramada.Data;
using CompraProgramada.Domain.Interface;
using Confluent.Kafka;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CompraProgramada.Shared.Exceptions.Base;
using CompraProgramada.Application.Contract.Service;

namespace CompraProgramada.Infra.Tests;

public class AppConfigurationTests
{
    private readonly ServiceCollection _services;
    private readonly Dictionary<string, string> _appSettings = new Dictionary<string, string>
    {
        { "ApplicationConfig:MotorCompraConfig:DiasDeCompra:0", "5" },
        { "ApplicationConfig:MotorCompraConfig:DiasDeCompra:1", "15" },
        { "ApplicationConfig:MotorCompraConfig:DiasDeCompra:2", "25" },
        { "ApplicationConfig:MotorCompraConfig:TempoEmHoraAhCadaExecucao", "10" },
        { "ApplicationConfig:MotorCompraConfig:NomePastaArquivosB3", "cotacoes" },
        { "Service:DataBase:ConnectionString", "Server=fake;" },
        { "Service:Kafka:Server", "localhost:9092" },
        { "Service:Kafka:SendMaxRetries", "5" }
    };

    public AppConfigurationTests() => _services = new ServiceCollection();

    [Fact]
    public async Task Deve_ConfigurarServicosApi_SemFalhas()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(_appSettings!)
            .Build();

        _services.AddLogging();

        MySqlServerVersion fakerVersion = new(new Version(8, 0, 0));

        _services.ConfigurarServicosApi(configuration, fakerVersion);

        var provider = _services.BuildServiceProvider();

        // Act
        var context = provider.GetRequiredService<AppDbContext>();
        var mediator = provider.GetRequiredService<IMediator>();
        var kafka = provider.GetRequiredService<IKafkaProducer>();
        var appConfig = provider.GetService<AppConfig>();

        // Assert
        Assert.NotNull(context);
        Assert.NotNull(mediator);
        Assert.NotNull(kafka);
        Assert.NotNull(appConfig);
    }

    [Fact]
    public async Task Deve_ConfigurarServicosWorker_SemFalhas()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(_appSettings!)
            .Build();

        _services.AddLogging();

        MySqlServerVersion fakerVersion = new(new Version(8, 0, 0));

        _services.ConfigurarServicosWorker(configuration, fakerVersion);

        var provider = _services.BuildServiceProvider();

        // Act
        var context = provider.GetRequiredService<AppDbContext>();
        var kafka = provider.GetRequiredService<IKafkaProducer>();
        var appConfig = provider.GetService<AppConfig>();

        // Assert
        Assert.NotNull(context);
        Assert.NotNull(kafka);
        Assert.NotNull(appConfig);
    }

    [Fact]
    public async Task Deve_Registrar_DbContext()
    {
        // Arrange
        var connectionString = "Server=localhost;Database=test;User=root;Password=123;";

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "Service:DataBase:ConnectionString", connectionString }
            }!)
            .Build();

        _services.AddLogging();

        MySqlServerVersion fakerVersion = new(new Version(8, 0, 0));

        _services.ConfigurarBancoDeDados(configuration, fakerVersion);

        var provider = _services.BuildServiceProvider();

        // Act
        var context = provider.GetRequiredService<AppDbContext>();

        // Assert
        Assert.NotNull(context);
        Assert.Equal("Pomelo.EntityFrameworkCore.MySql", context.Database.ProviderName);
    }

    [Fact]
    public async Task Deve_Resolver_Mediatr()
    {
        // Arrange
        _services.AddLogging();
        _services.ConfigurarMediatR();

        var provider = _services.BuildServiceProvider();

        // Act
        var mediator = provider.GetService<IMediator>();

        // Assert
        Assert.NotNull(mediator);
    }

    [Fact]
    public async Task Repository_Deve_Ser_Scoped()
    {
        // Arrange
        _services.AdicionaServicosERepositorios();

        // Act
        var cestaDescriptor = _services.FirstOrDefault(s => s.ServiceType == typeof(ICestaRecomendadaRepository));
        var clienteDescriptor = _services.FirstOrDefault(s => s.ServiceType == typeof(IClienteRepository));
        var contaGraficaDescriptor = _services.FirstOrDefault(s => s.ServiceType == typeof(IContaGraficaRepository));
        var contaMasterDescriptor = _services.FirstOrDefault(s => s.ServiceType == typeof(IContaMasterRepository));
        var cotacaoDescriptor = _services.FirstOrDefault(s => s.ServiceType == typeof(ICotacaoRepository));
        var custodiaFilhoteDescriptor = _services.FirstOrDefault(s => s.ServiceType == typeof(ICustodiaFilhoteRepository));
        var custodiaMasterDescriptor = _services.FirstOrDefault(s => s.ServiceType == typeof(ICustodiaMasterRepository));
        var historicoExecucaoDescriptor = _services.FirstOrDefault(s => s.ServiceType == typeof(IHistoricoExecucaoMotorRepository));
        var ordemCompraDescriptor = _services.FirstOrDefault(s => s.ServiceType == typeof(IOrdemCompraRepository));

        // Assert
        Assert.NotNull(cestaDescriptor);
        Assert.Equal(ServiceLifetime.Scoped, cestaDescriptor?.Lifetime);
        Assert.NotNull(clienteDescriptor);
        Assert.Equal(ServiceLifetime.Scoped, clienteDescriptor?.Lifetime);
        Assert.NotNull(contaGraficaDescriptor);
        Assert.Equal(ServiceLifetime.Scoped, contaGraficaDescriptor?.Lifetime);
        Assert.NotNull(contaMasterDescriptor);
        Assert.Equal(ServiceLifetime.Scoped, contaMasterDescriptor?.Lifetime);
        Assert.NotNull(cotacaoDescriptor);
        Assert.Equal(ServiceLifetime.Scoped, cotacaoDescriptor?.Lifetime);
        Assert.NotNull(custodiaFilhoteDescriptor);
        Assert.Equal(ServiceLifetime.Scoped, custodiaFilhoteDescriptor?.Lifetime);
        Assert.NotNull(custodiaMasterDescriptor);
        Assert.Equal(ServiceLifetime.Scoped, custodiaMasterDescriptor?.Lifetime);
        Assert.NotNull(historicoExecucaoDescriptor);
        Assert.Equal(ServiceLifetime.Scoped, historicoExecucaoDescriptor?.Lifetime);
        Assert.NotNull(ordemCompraDescriptor);
        Assert.Equal(ServiceLifetime.Scoped, ordemCompraDescriptor?.Lifetime);
    }

    [Fact]
    public async Task Service_Deve_Ser_Scoped()
    {
        // Arrange
        _services.AdicionaServicosERepositorios();

        // Act
        var cestaDescriptor = _services.FirstOrDefault(s => s.ServiceType == typeof(ICestaRecomendadaService));
        var clienteDescriptor = _services.FirstOrDefault(s => s.ServiceType == typeof(IClienteService));
        var contaGraficaDescriptor = _services.FirstOrDefault(s => s.ServiceType == typeof(IContaGraficaService));
        var cotacaoDescriptor = _services.FirstOrDefault(s => s.ServiceType == typeof(ICotacaoService));
        var custodiaMasterDescriptor = _services.FirstOrDefault(s => s.ServiceType == typeof(ICustodiaMasterService));
        var distribuicaoMasterDescriptor = _services.FirstOrDefault(s => s.ServiceType == typeof(IDistribuicaoService));
        var historicoExecucaoDescriptor = _services.FirstOrDefault(s => s.ServiceType == typeof(IHistoricoExecucaoMotorService));
        var compraDescriptor = _services.FirstOrDefault(s => s.ServiceType == typeof(ICompraService));
        var ordemCompraDescriptor = _services.FirstOrDefault(s => s.ServiceType == typeof(IOrdemCompraService));

        // Assert
        Assert.NotNull(cestaDescriptor);
        Assert.Equal(ServiceLifetime.Scoped, cestaDescriptor?.Lifetime);
        Assert.NotNull(clienteDescriptor);
        Assert.Equal(ServiceLifetime.Scoped, clienteDescriptor?.Lifetime);
        Assert.NotNull(contaGraficaDescriptor);
        Assert.Equal(ServiceLifetime.Scoped, contaGraficaDescriptor?.Lifetime);
        Assert.NotNull(cotacaoDescriptor);
        Assert.Equal(ServiceLifetime.Scoped, cotacaoDescriptor?.Lifetime);
        Assert.NotNull(custodiaMasterDescriptor);
        Assert.Equal(ServiceLifetime.Scoped, custodiaMasterDescriptor?.Lifetime);
        Assert.NotNull(historicoExecucaoDescriptor);
        Assert.Equal(ServiceLifetime.Scoped, historicoExecucaoDescriptor?.Lifetime);
        Assert.NotNull(ordemCompraDescriptor);
        Assert.Equal(ServiceLifetime.Scoped, ordemCompraDescriptor?.Lifetime);
    }

    [Fact]
    public async Task Service_Deve_Ser_Singleton()
    {
        // Arrange
        _services.AdicionaServicosERepositorios();

        // Act
        var cotahistDescriptor = _services.FirstOrDefault(s => s.ServiceType == typeof(ICotahistParserService));
        var calendarioDescriptor = _services.FirstOrDefault(s => s.ServiceType == typeof(ICalendarioMotorCompraService));
        var fileSysDescriptor = _services.FirstOrDefault(s => s.ServiceType == typeof(IFileSystem));
        var impostoRendaDescriptor = _services.FirstOrDefault(s => s.ServiceType == typeof(IImpostoRendaService));
        var datetimeProvaiderDescriptor = _services.FirstOrDefault(s => s.ServiceType == typeof(IDateTimeProvaider));

        // Assert
        Assert.NotNull(cotahistDescriptor);
        Assert.Equal(ServiceLifetime.Singleton, cotahistDescriptor?.Lifetime);
        Assert.NotNull(calendarioDescriptor);
        Assert.Equal(ServiceLifetime.Singleton, calendarioDescriptor?.Lifetime);
        Assert.NotNull(fileSysDescriptor);
        Assert.Equal(ServiceLifetime.Singleton, fileSysDescriptor?.Lifetime);
        Assert.NotNull(impostoRendaDescriptor);
        Assert.Equal(ServiceLifetime.Singleton, impostoRendaDescriptor?.Lifetime);
        Assert.NotNull(datetimeProvaiderDescriptor);
        Assert.Equal(ServiceLifetime.Singleton, datetimeProvaiderDescriptor?.Lifetime);
    }

    [Fact]
    public async Task Deve_Registrar_Validators()
    {
        // Arrange
        _services.ConfigurarFluentValidation();

        var provider = _services.BuildServiceProvider();

        // Act
        var validatorAdesao = provider.GetService<IValidator<AdesaoRequest>>();
        var validatorValorMensal = provider.GetService<IValidator<AtualizarValorMensalRequest>>();
        var validatorComposicaoCesta = provider.GetService<IValidator<ComposicaoCestaDto>>();
        var validatorCesta = provider.GetService<IValidator<CriarCestaRecomendadaRequest>>();

        // Assert
        Assert.NotNull(validatorAdesao);
        Assert.NotNull(validatorValorMensal);
        Assert.NotNull(validatorComposicaoCesta);
        Assert.NotNull(validatorCesta);
    }

    [Fact]
    public async Task Deve_Configurar_AppConfig()
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string>
        {
            { "ApplicationConfig:MotorCompraConfig:DiasDeCompra:0", "2" },
            { "ApplicationConfig:MotorCompraConfig:DiasDeCompra:1", "7" },
            { "ApplicationConfig:MotorCompraConfig:DiasDeCompra:2", "75" },
            { "ApplicationConfig:MotorCompraConfig:TempoEmHoraAhCadaExecucao", "23" },
            { "ApplicationConfig:MotorCompraConfig:NomePastaArquivosB3", "directory" }
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();

        _services.ConfigurarRegrasDaAplicacao(configuration);

        var provider = _services.BuildServiceProvider();

        // Act
        var config = provider.GetService<AppConfig>();

        // Assert
        Assert.NotNull(config);
        Assert.NotNull(config?.MotorCompraConfig);
        Assert.Equal(new List<int> { 2, 7, 75 }, config?.MotorCompraConfig.DiasDeCompra);
        Assert.Equal(23, config?.MotorCompraConfig.TempoEmHoraAhCadaExecucao);
        Assert.Equal("directory", config?.MotorCompraConfig.NomePastaArquivosB3);
    }

    [Fact]
    public async Task Deve_Configurar_Kafka()
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string>
        {
            { "Service:Kafka:Server", "https://test.kafka.com" },
            { "Service:Kafka:SendMaxRetries", "15" }
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();

        _services.ConfigurarKafka(configuration);

        var provider = _services.BuildServiceProvider();

        // Act
        var config = provider.GetService<ProducerConfig>();
        var producer = provider.GetService<IKafkaProducer>();

        // Assert
        Assert.NotNull(config);
        Assert.Equal("https://test.kafka.com", config?.BootstrapServers);
        Assert.Equal(15, config?.MessageSendMaxRetries);
        Assert.NotNull(producer);
        Assert.IsType<KafkaProducer>(producer);
    }

    [Fact]
    public async Task Deve_Tratar_DomainExceptionHandler()
    {
        // Arrange
        _services.AddLogging();
        _services.ConfigurarExceptionHandler();

        var provider = _services.BuildServiceProvider();

        var handler = provider.GetService<IExceptionHandler>();

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var exception = new DomainException("Erro de domínio", "COD123");

        // Act
        var handled = await handler!.TryHandleAsync(
            context,
            exception,
            CancellationToken.None);

        // Assert
        Assert.True(handled);
        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
    }
}