using CompraProgramada.Domain.Contract.Repository;
using CompraProgramada.Domain.Contract.Service;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Handler.Api;
using CompraProgramada.Domain.Mapper;
using CompraProgramada.Domain.Tests.TestUtils;
using CompraProgramada.Shared.Dto;
using CompraProgramada.Shared.Exceptions;
using CompraProgramada.Shared.Request;
using CompraProgramada.Shared.Response;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace CompraProgramada.Domain.Tests.Handler;

public class ClienteHandlerTests
{
    private readonly ILogger<ClienteHandle> _loggerMock;
    private readonly IClienteRepository _clienteRepository;
    private readonly ICestaRecomendadaRepository _cestaRecomendadaRepository;
    private readonly ICotacaoService _cotacaoService;
    private readonly ClienteMapper _mapper;
    private readonly ClienteHandle _sut;

    public ClienteHandlerTests()
    {
        _loggerMock = Substitute.For<ILogger<ClienteHandle>>();
        _clienteRepository = Substitute.For<IClienteRepository>();
        _cestaRecomendadaRepository = Substitute.For<ICestaRecomendadaRepository>();
        _cotacaoService = Substitute.For<ICotacaoService>();
        _mapper = Substitute.For<ClienteMapper>(Substitute.For<ContaMapper>());
        _sut = new ClienteHandle(_loggerMock, _clienteRepository, _cestaRecomendadaRepository, _cotacaoService, _mapper);
    }

    [Fact]
    public async Task Handle_Deve_AderirCliente_ERetornarSucesso_Quando_NaoHouverErro()
    {
        var request = FakerRequest.AdesaoRequest().Generate();
        var response = FakerRequest.ClienteAtivo().Generate();

        _clienteRepository.CpfExistenteAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(false);

        _cestaRecomendadaRepository.ObterCestaAtualAsync(Arg.Any<CancellationToken>())
            .Returns(CestaRecomendada.CriarCesta("", [.. FakerRequest.ComposicaoCestaRecomendada().Select(x => ComposicaoCesta.CriaItemNaCesta(x.Ticker, x.Percentual))]));

        _clienteRepository
            .CriarAsync(Arg.Any<Cliente>(), Arg.Any<CancellationToken>())
            .Returns(response);

        _clienteRepository.CriarContaAsync(Arg.Any<ContaGrafica>(), Arg.Any<CancellationToken>())
            .Returns(ContaGrafica.Gerar(Cliente.Criar(request)));

        var resultado = await _sut.Handle(request, CancellationToken.None);

        var data = resultado.Value;

        resultado.IsSuccess.Should().BeTrue();
        resultado.Exception.Should().BeNull();
        data.Should().BeOfType<AdesaoResponse>();
        data.Ativo.Should().BeTrue();
        data.Nome.Should().Be(request.Nome);
        data.Cpf.Should().Be(request.Cpf);
        data.Email.Should().Be(request.Email);
        data.ValorMensal.Should().Be(request.ValorMensal);
    }

    [Fact]
    public async Task Handle_Deve_RetornarCpfExistenteException_Quando_CpfJaExistente()
    {
        var request = FakerRequest.AdesaoRequest().Generate();
        var exception = new CpfExistenteException();

        _clienteRepository.CpfExistenteAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(true);

        var resultado = await _sut.Handle(request, CancellationToken.None);
        
        resultado.IsSuccess.Should().BeFalse();
        resultado.Exception.Should().BeEquivalentTo(exception);
    }

    [Fact]
    public async Task Handle_Deve_RetornarAppException_Quando_NaoTiverCestaAtiva()
    {
        var request = FakerRequest.AdesaoRequest().Generate();
        var exception = new AppException("Adesão não pode ser realizada", "CESTA_NAO_ENCONTRADA");

        _clienteRepository.CpfExistenteAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(false);

        _cestaRecomendadaRepository.ObterCestaAtualAsync(Arg.Any<CancellationToken>())
            .Returns((CestaRecomendada)null!);

        var resultado = await _sut.Handle(request, CancellationToken.None);

        resultado.IsSuccess.Should().BeFalse();
        resultado.Exception.Should().BeEquivalentTo(exception);
    }

    [Fact]
    public async Task Handle_Deve_RetirarClienteDoProduto_ERetornarSucesso_Quando_NaoHouverErro()
    {
        var request = new SaidaProdutoRequest(1);
        var cliente = FakerRequest.ClienteAtivo().Generate();

        _clienteRepository.ObterAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(cliente);

        _clienteRepository.AtualizarAsync(Arg.Any<Cliente>(), Arg.Any<CancellationToken>())
            .Returns(cliente);

        var resultado = await _sut.Handle(request, CancellationToken.None);

        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().BeOfType<SaidaProdutoResponse>();
        resultado.Value.Ativo.Should().BeFalse();
        resultado.Value.Mensagem.Should().Be("Adesao encerrada. Sua posicao em custodia foi mantida.");
    }

    [Fact]
    public async Task Handle_Deve_RetornarAppException_Quando_ClienteComStatusInativo()
    {
        var request = new SaidaProdutoRequest(1);
        var cliente = FakerRequest.ClienteAtivo().Generate();
        cliente.Desativar();
        var exception = new AppException("Cliente já está inativo", "CLIENTE_INATIVO");

        _clienteRepository.ObterAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(cliente);

        var resultado = await _sut.Handle(request, CancellationToken.None);

        resultado.IsSuccess.Should().BeFalse();
        resultado.Exception.Should().BeEquivalentTo(exception);
    }

    [Fact]
    public async Task Handle_Deve_AtualizarValorMensal_ERetornarSucesso_Quando_NaoHouverErro()
    {
        var request = new AtualizarValorMensalRequest(1, 50000);
        var cliente = FakerRequest.ClienteAtivo().Generate();

        _clienteRepository.ObterAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(cliente);

        _clienteRepository.AtualizarAsync(Arg.Any<Cliente>(), Arg.Any<CancellationToken>())
            .Returns(cliente);

        var resultado = await _sut.Handle(request, CancellationToken.None);

        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().BeOfType<AtualizarValorMensalResponse>();
    }

    [Fact]
    public async Task Handle_Deve_RetornarAppException_Quando_ClienteComStatusInativo_AoAlterarValorMensal()
    {
        var request = new AtualizarValorMensalRequest(1, 100);
        var exception = new AppException("Cliente já está inativo", "CLIENTE_INATIVO");
        var cliente = FakerRequest.ClienteAtivo().Generate();
        cliente.Desativar();

        _clienteRepository.ObterAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(cliente);

        var resultado = await _sut.Handle(request, CancellationToken.None);
        
        resultado.IsSuccess.Should().BeFalse();
        resultado.Exception.Should().BeEquivalentTo(exception);
    }

    [Fact]
    public async Task Handle_DeveRetornarSucesso_Quando_CarteiraConsultada()
    {
        // Arrange
        var request = new CarteiraCustodiaRequest(1);
        var cliente = FakerRequest.ClienteAtivo().Generate();
        ContaGrafica conta = new(1, "", DateTime.MinValue, cliente, new() { }, new() { new(1, 1, "PETR4", 28.59m, 150), new(1, 1, "VALE3", 72.10m, 80), new(1, 1, "ITUB4", 26.40m, 300), new(1, 1, "WEGE3", 38.15m, 120), new(1, 1, "MGLU3", 5.20m, 1000) }, new() { });
        cliente.AdicionarConta(conta);
        CestaRecomendada cestaAtiva = CestaRecomendada.CriarCesta("Cesta", new()
        {
            ComposicaoCesta.CriaItemNaCesta("PETR4", 30),
            ComposicaoCesta.CriaItemNaCesta("VALE3", 25),
            ComposicaoCesta.CriaItemNaCesta("ITUB4", 20),
            ComposicaoCesta.CriaItemNaCesta("BBDC4", 15),
            ComposicaoCesta.CriaItemNaCesta("MGLU3", 10)
        });
        CarteiraCustodiaResponse resultadoEsperado = new(0,
            cliente.Nome,
            conta.NumeroConta,
            DateTime.Now,
            new ResumoCarteiraDto
            {
                ValorAtualCarteira = 27872,
                ValorTotalInvestido = 27754.50m,
                PlTotal = 117.50m,
                RentabilidadePercentual = 0.42m
            },
            new()
            {
                new DetalheCarteiraDto { Ticker = "PETR4", Quantidade = 150, PrecoMedio = 28.59m, CotacaoAtual = 36.50m, ValorAtual = 5475, Pl = 1186.50m, PlPercentual = 27.67m, ComposicaoCarteira = 19.64m },
                new DetalheCarteiraDto { Ticker  = "VALE3", Quantidade = 80, PrecoMedio = 72.10m, CotacaoAtual = 68.20m, ValorAtual = 5456, Pl = -312.00m, PlPercentual = -5.41m, ComposicaoCarteira = 19.58m },
                new DetalheCarteiraDto { Ticker = "ITUB4", Quantidade = 300, PrecoMedio = 26.40m, CotacaoAtual = 33.15m, ValorAtual = 9945, Pl = 2025m, PlPercentual = 25.57m, ComposicaoCarteira = 35.68m  },
                new DetalheCarteiraDto { Ticker = "WEGE3", Quantidade = 120, PrecoMedio = 38.15m, CotacaoAtual = 42.05m, ValorAtual = 5046, Pl = 468m, PlPercentual = 10.22m, ComposicaoCarteira = 18.10m  },
                new DetalheCarteiraDto { Ticker = "MGLU3", Quantidade = 1000, PrecoMedio = 5.20m, CotacaoAtual = 1.95m, ValorAtual = 1950, Pl = -3250m, PlPercentual = -62.50m, ComposicaoCarteira = 7.00m }
            });

        _clienteRepository.ObterAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())!
            .Returns(cliente);

        _cestaRecomendadaRepository.ObterCestaAtualAsync(Arg.Any<CancellationToken>())
            .Returns(cestaAtiva);

        _cotacaoService.ObterCotacoesDaCestaRecomendadaAsync(Arg.Any<CestaRecomendada>(), Arg.Any<CancellationToken>())!
            .Returns(new Cotacao(1, DateOnly.MinValue, DateTime.Now, new() { ComposicaoCotacao.CriarItem("PETR4", 36.50m), ComposicaoCotacao.CriarItem("VALE3", 68.20m), ComposicaoCotacao.CriarItem("ITUB4", 33.15m), ComposicaoCotacao.CriarItem("WEGE3", 42.05m), ComposicaoCotacao.CriarItem("MGLU3", 1.95m) }));

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Exception.Should().BeNull();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEquivalentTo(resultadoEsperado, options =>
            options.Using<DateTime>(ctx =>
                ctx.Subject.Date.Should().Be(ctx.Expectation.Date)
            ).WhenTypeIs<DateTime>());
    }

    [Fact]
    public async Task Handle_Deve_RetornarAppException_Quando_CestaVigenteNaoEncontrada_AoConsultarCarteira()
    {
        var request = new CarteiraCustodiaRequest(1);
        var cliente = FakerRequest.ClienteAtivo().Generate();
        var exception = new AppException("Nenhuma cesta vigente encontrada", "CESTA_NAO_ENCONTRADA");

        _clienteRepository.ObterAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())!
            .Returns(cliente);

        _cestaRecomendadaRepository.ObterCestaAtualAsync(Arg.Any<CancellationToken>())
            .Returns((CestaRecomendada)null!);

        var resultado = await _sut.Handle(request, CancellationToken.None);

        resultado.IsSuccess.Should().BeFalse();
        resultado.Exception.Should().BeEquivalentTo(exception);
    }

    [Fact]
    public async Task Handle_Deve_RetornarException_Quando_ObterCotacaoFalhar_AoConsultarCarteira()
    {
        var request = new CarteiraCustodiaRequest(1);
        var cliente = FakerRequest.ClienteAtivo().Generate();
        var exception = new Exception("Nenhuma cesta vigente encontrada");

        _clienteRepository.ObterAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())!
            .Returns(cliente);

        _cestaRecomendadaRepository.ObterCestaAtualAsync(Arg.Any<CancellationToken>())
            .Returns((CestaRecomendada)null!);

        _cotacaoService.ObterCotacoesDaCestaRecomendadaAsync(Arg.Any<CestaRecomendada>(), Arg.Any<CancellationToken>())!
            .Returns(exception);

        var resultado = await _sut.Handle(request, CancellationToken.None);

        resultado.IsSuccess.Should().BeFalse();
        resultado.Exception.Should().BeEquivalentTo(exception);
    }

    [Fact]
    public async Task Handle_DeveRetornarSucesso_Quando_RentabilidadeConsultada()
    {
        // Arrange
        var request = new RentabilidadeRequest(1);
        var cliente = FakerRequest.ClienteAtivo().Generate();
        ContaGrafica conta = new(1, "", DateTime.MinValue, cliente, new() { },
            new() { new(1, 1, "PETR4", 28.59m, 150), new(1, 1, "VALE3", 72.10m, 80), new(1, 1, "ITUB4", 26.40m, 300), new(1, 1, "WEGE3", 38.15m, 120), new(1, 1, "MGLU3", 5.20m, 1000) },
            new() { new(1, 1, "PETR4", 150, 36, 1, 1, DateOnly.FromDateTime(new DateTime(2026, 03, 05))) });
        cliente.AdicionarConta(conta);
        CestaRecomendada cestaAtiva = CestaRecomendada.CriarCesta("Cesta", new()
        {
            ComposicaoCesta.CriaItemNaCesta("PETR4", 30),
            ComposicaoCesta.CriaItemNaCesta("VALE3", 25),
            ComposicaoCesta.CriaItemNaCesta("ITUB4", 20),
            ComposicaoCesta.CriaItemNaCesta("BBDC4", 15),
            ComposicaoCesta.CriaItemNaCesta("MGLU3", 10)
        });
        RentabilidadeResponse resultadoEsperado = new(0, cliente.Nome, DateTime.Now,
            new ResumoCarteiraDto
            {
                ValorAtualCarteira = 27872,
                ValorTotalInvestido = 27754.50m,
                PlTotal = 117.50m,
                RentabilidadePercentual = 0.42m
            },
            new() { new HistoricoAporteDto { Parcela = "1/3", Valor = 1, Data = DateOnly.FromDateTime(new DateTime(2026, 03, 05)) } },
            new() { new EvolucaoCarteiraDto { Rentabilidade = 547400, ValorCarteira = 5475, ValorInvestido = 1, Data = DateOnly.FromDateTime(new DateTime(2026, 03, 05)) } });

        _clienteRepository.ObterAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())!
            .Returns(cliente);

        _cestaRecomendadaRepository.ObterCestaAtualAsync(Arg.Any<CancellationToken>())
            .Returns(cestaAtiva);

        _cotacaoService.ObterCotacoesDaCestaRecomendadaAsync(Arg.Any<CestaRecomendada>(), Arg.Any<CancellationToken>())!
            .Returns(new Cotacao(1, DateOnly.MinValue, DateTime.Now, new() { ComposicaoCotacao.CriarItem("PETR4", 36.50m), ComposicaoCotacao.CriarItem("VALE3", 68.20m), ComposicaoCotacao.CriarItem("ITUB4", 33.15m), ComposicaoCotacao.CriarItem("WEGE3", 42.05m), ComposicaoCotacao.CriarItem("MGLU3", 1.95m) }));

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Exception.Should().BeNull();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEquivalentTo(resultadoEsperado, options =>
            options.Using<DateTime>(ctx =>
                ctx.Subject.Date.Should().Be(ctx.Expectation.Date)
            ).WhenTypeIs<DateTime>());
    }

    [Fact]
    public async Task Handle_Deve_RetornarAppException_Quando_CestaVigenteNaoEncontrada_AoConsultarRentabilidade()
    {
        var request = new RentabilidadeRequest(1);
        var cliente = FakerRequest.ClienteAtivo().Generate();
        var exception = new AppException("Nenhuma cesta vigente encontrada", "CESTA_NAO_ENCONTRADA");

        _clienteRepository.ObterAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())!
            .Returns(cliente);

        _cestaRecomendadaRepository.ObterCestaAtualAsync(Arg.Any<CancellationToken>())
            .Returns((CestaRecomendada)null!);

        var resultado = await _sut.Handle(request, CancellationToken.None);
        
        resultado.IsSuccess.Should().BeFalse();
        resultado.Exception.Should().BeEquivalentTo(exception);
    }

    [Fact]
    public async Task Handle_Deve_RetornarException_Quando_ObterCotacaoFalhar_AoConsultarRentabilidade()
    {
        var request = new RentabilidadeRequest(1);
        var cliente = FakerRequest.ClienteAtivo().Generate();
        var exception = new Exception("Nenhuma cesta vigente encontrada");

        _clienteRepository.ObterAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())!
            .Returns(cliente);

        _cestaRecomendadaRepository.ObterCestaAtualAsync(Arg.Any<CancellationToken>())
            .Returns((CestaRecomendada)null!);

        _cotacaoService.ObterCotacoesDaCestaRecomendadaAsync(Arg.Any<CestaRecomendada>(), Arg.Any<CancellationToken>())!
            .Returns(exception);

        var resultado = await _sut.Handle(request, CancellationToken.None);

        resultado.IsSuccess.Should().BeFalse();
        resultado.Exception.Should().BeEquivalentTo(exception);
    }
}