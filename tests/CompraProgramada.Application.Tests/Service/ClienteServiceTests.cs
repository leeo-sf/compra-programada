using CompraProgramada.Application.Contract.Service;
using CompraProgramada.Application.Mapper;
using CompraProgramada.Application.Service;
using CompraProgramada.Application.Tests.TestUtils;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;
using CompraProgramada.Shared.Dto;
using CompraProgramada.Shared.Exceptions;
using CompraProgramada.Shared.Request;
using CompraProgramada.Shared.Response;
using FluentAssertions;
using NSubstitute;
using OperationResult;

namespace CompraProgramada.Application.Tests.Service;

public class ClienteServiceTests
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IContaGraficaService _contaGraficaService;
    private readonly ICestaRecomendadaService _cestaRecomendadaService;
    private readonly ICotacaoService _cotacaoService;
    private readonly ClienteMapper _mapper;
    private readonly ClienteService _sut;

    public ClienteServiceTests()
    {
        _clienteRepository = Substitute.For<IClienteRepository>();
        _contaGraficaService = Substitute.For<IContaGraficaService>();
        _cestaRecomendadaService = Substitute.For<ICestaRecomendadaService>();
        _cotacaoService = Substitute.For<ICotacaoService>();
        _mapper = Substitute.For<ClienteMapper>(Substitute.For<ContaMapper>());
        _sut = new(_clienteRepository, _contaGraficaService, _cestaRecomendadaService, _cotacaoService, _mapper);
    }

    [Fact]
    public async Task ClienteService_Deve_RetornarClientesAtivos_Quando_TiverClientesAtivos()
    {
        // Arrange
        var response = FakerRequest.ClientesAtivos().Generate();
        _clienteRepository.ObterClientesAtivosAsync(Arg.Any<CancellationToken>())
            .Returns(response);

        // Act
        var result = await _sut.ObtemClientesAtivoAsync(CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Exception.Should().BeNull();
        result.Value.Should().NotBeNullOrEmpty();
        result.Value.Should().OnlyContain(x => x.Ativo == true);
    }

    [Fact]
    public async Task ClienteService_Deve_RetornarListaVazia_Quando_NaoHouver_ClientesAtivos()
    {
        // Arrange
        _clienteRepository.ObterClientesAtivosAsync(Arg.Any<CancellationToken>())
            .Returns((List<Cliente>)null!);

        // Act
        var result = await _sut.ObtemClientesAtivoAsync(CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Exception.Should().BeNull();
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task ClienteService_Deve_RealizarAdesaoComSucesso_Quando_NaoOcorrerFalhas()
    {
        // Arrange
        var cliente = FakerRequest.ClienteAtivo().Generate();
        var command = FakerRequest.AdesaoRequest().Generate();
        var cestaRequest = FakerRequest.CriarCestaRecomendadaRequest();
        var cestaAtiva = CestaRecomendada.CriarCesta(cestaRequest.Nome, cestaRequest.Itens.Select(x => ComposicaoCesta.CriaItemNaCesta(x.Ticker, x.Percentual)).ToList());

        _clienteRepository.ExisteAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(false);
        
        _cestaRecomendadaService.ObterCestaAtivaAsync(Arg.Any<CancellationToken>())
            .Returns(cestaAtiva);

        _clienteRepository.CriarAsync(Arg.Any<Cliente>(), Arg.Any<CancellationToken>())
            .Returns(Cliente.Criar(new(command.Nome, command.Cpf, command.Email, command.ValorMensal)));

        _contaGraficaService.GerarContaGraficaAsync(Arg.Any<Cliente>(), Arg.Any<CancellationToken>())
            .Returns(ContaGrafica.Gerar(cliente));

        // Act
        var result = await _sut.RealizarAdesaoAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Exception.Should().BeNull();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeOfType<Cliente>();
        result.Value.ContaGrafica.Should().NotBeNull();
        result.Value.Ativo.Should().BeTrue();
    }

    [Fact]
    public async Task ClienteService_Deve_Falhar_AoAdesao_Quando_CpfExistirNoSistema()
    {
        // Arrange
        var command = FakerRequest.AdesaoRequest().Generate();
        var cestaRequest = FakerRequest.CriarCestaRecomendadaRequest();
        var cestaAtiva = CestaRecomendada.CriarCesta(cestaRequest.Nome, cestaRequest.Itens.Select(x => ComposicaoCesta.CriaItemNaCesta(x.Ticker, x.Percentual)).ToList());

        _clienteRepository.ExisteAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        var result = await _sut.RealizarAdesaoAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Exception.Should().BeOfType<CpfExistenteException>();
        result.Exception.Message.Should().Be("CPF ja cadastrado no sistema.");
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task ClienteService_Deve_Falhar_AoAdesao_Quando_NaoHouverCestaAtiva()
    {
        // Arrange
        var command = FakerRequest.AdesaoRequest().Generate();
        var cestaRequest = FakerRequest.CriarCestaRecomendadaRequest();
        var cestaAtiva = CestaRecomendada.CriarCesta(cestaRequest.Nome, cestaRequest.Itens.Select(x => ComposicaoCesta.CriaItemNaCesta(x.Ticker, x.Percentual)).ToList());

        _clienteRepository.ExisteAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(false);

        _cestaRecomendadaService.ObterCestaAtivaAsync(Arg.Any<CancellationToken>())!
            .Returns(Result.Error<CestaRecomendada>(new ApplicationException("Nenhuma cesta ativa")));

        // Act
        var result = await _sut.RealizarAdesaoAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Exception.Should().BeOfType<ApplicationException>();
        result.Exception.Message.Should().Be("Nenhuma cesta ativa");
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task ClienteService_Deve_Falhar_AoAdesao_Quando_GerarContaFalhar()
    {
        // Arrange
        var command = FakerRequest.AdesaoRequest().Generate();
        var cestaRequest = FakerRequest.CriarCestaRecomendadaRequest();
        var cestaAtiva = CestaRecomendada.CriarCesta(cestaRequest.Nome, cestaRequest.Itens.Select(x => ComposicaoCesta.CriaItemNaCesta(x.Ticker, x.Percentual)).ToList());

        _clienteRepository.ExisteAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(false);

        _cestaRecomendadaService.ObterCestaAtivaAsync(Arg.Any<CancellationToken>())
            .Returns(cestaAtiva);

        _contaGraficaService.GerarContaGraficaAsync(Arg.Any<Cliente>(), Arg.Any<CancellationToken>())!
            .Returns(Result.Error<ContaGrafica>(new ApplicationException("Falha ao gerar conta gráfica")));

        // Act
        var result = await _sut.RealizarAdesaoAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Exception.Should().BeOfType<ApplicationException>();
        result.Exception.Message.Should().Be("Falha ao gerar conta gráfica");
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task ClienteService_Deve_RetornarQuantidadeClientesAtivos_Quando_Solicitado()
    {
        // Arrange
        var clientesAtivos = FakerRequest.ClientesAtivos().Generate();

        _clienteRepository.QuantidadeAtivosAsync(Arg.Any<CancellationToken>())!
            .Returns(clientesAtivos.Count);

        // Act
        var result = await _sut.QuantidadeClientesAtivosAsync(CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Exception.Should().BeNull();
        result.Value.Should().Be(clientesAtivos.Count);
    }

    [Fact]
    public async Task ClienteService_Deve_DesativarCliente_Quando_SolicitarSaidaDoProduto()
    {
        // Arrange
        var cliente = FakerRequest.ClienteAtivo().Generate();

        _clienteRepository.ObterClienteAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())!
            .Returns(cliente);

        _clienteRepository.AtualizarClienteAsync(Arg.Any<Cliente>(), Arg.Any<CancellationToken>())!
            .Returns(cliente);

        // Act
        var result = await _sut.SairDoProdutoAsync(cliente.Id, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Exception.Should().BeNull();
        result.Value.Should().NotBeNull();
        result.Value.Ativo.Should().BeFalse();
    }

    [Fact]
    public async Task ClienteService_Deve_RetornarClienteNaoEncontradoException_Quando_SairProduto_Ao_ClienteIdNaoEncontrado()
    {
        // Arrange
        var cliente = FakerRequest.ClienteAtivo().Generate();

        _clienteRepository.ObterClienteAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())!
            .Returns((Cliente)null!);

        // Act
        var result = await _sut.SairDoProdutoAsync(cliente.Id, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Exception.Should().BeOfType<ClienteNaoEncontradoException>();
        result.Exception.Message.Should().Be("Cliente nao encontrado.");
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task ClienteService_Deve_RetornarApplicationException_Quando_SairProduto_Ao_ClienteJaEstaInativo()
    {
        // Arrange
        var cliente = FakerRequest.ClienteAtivo().Generate();
        cliente.Desativar();

        _clienteRepository.ObterClienteAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())!
            .Returns(cliente);

        // Act
        var result = await _sut.SairDoProdutoAsync(cliente.Id, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Exception.Should().BeOfType<ApplicationException>();
        result.Exception.Message.Should().Be("Cliente já está com status inativo");
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task ClienteService_Deve_AtualizarValorMensal_Quando_SolicitarAlteracao()
    {
        // Arrange
        var request = new AtualizarValorMensalRequest(1, 500);
        var cliente = FakerRequest.ClienteAtivo().Generate();

        _clienteRepository.ObterClienteAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())!
            .Returns(cliente);

        _clienteRepository.AtualizarClienteAsync(Arg.Any<Cliente>(), Arg.Any<CancellationToken>())!
            .Returns(cliente);

        // Act
        var result = await _sut.AtualizarValorMensalAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Exception.Should().BeNull();
        result.Value.Should().NotBeNull();
        result.Value.Ativo.Should().BeTrue();
        result.Value.ValorAnterior.Should().Be(cliente.ValorAnterior);
        result.Value.ValorMensal.Should().Be(500);
    }

    [Fact]
    public async Task ClienteService_Deve_RetornarClienteNaoEncontradoException_Quando_AtualizarValorMensal_Ao_ClienteIdNaoEncontrado()
    {
        // Arrange
        var request = new AtualizarValorMensalRequest(1, 500);

        _clienteRepository.ObterClienteAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())!
            .Returns((Cliente)null!);

        // Act
        var result = await _sut.AtualizarValorMensalAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Exception.Should().BeOfType<ClienteNaoEncontradoException>();
        result.Exception.Message.Should().Be("Cliente nao encontrado.");
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task ClienteService_Deve_RetornarApplicationException_Quando_AtualizarValorMensal_Ao_ClienteJaEstaInativo()
    {
        // Arrange
        var request = new AtualizarValorMensalRequest(1, 500);
        var cliente = FakerRequest.ClienteAtivo().Generate();
        cliente.Desativar();

        _clienteRepository.ObterClienteAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())!
            .Returns(cliente);

        // Act
        var result = await _sut.AtualizarValorMensalAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Exception.Should().BeOfType<ApplicationException>();
        result.Exception.Message.Should().Be("Cliente com status inativo");
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task ClienteService_Deve_RetornarClienteNaoEncontradoException_Quando_ConsultarCarteira()
    {
        // Arrange
        _clienteRepository.ObterClienteAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())!
            .Returns((Cliente)null!);

        // Act
        Task result() => _sut.ConsultarCarteiraAsync(1, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<ClienteNaoEncontradoException>(result);
    }

    [Fact]
    public async Task ClienteService_Deve_RetornarException_Quando_CestaAtivaFalhar()
    {
        // Arrange
        var cliente = FakerRequest.ClienteAtivo().Generate();

        _clienteRepository.ObterClienteAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())!
            .Returns(cliente);

        _cestaRecomendadaService.ObterCestaAtivaAsync(Arg.Any<CancellationToken>())
            .Returns(new ApplicationException());

        // Act
        var result = await _sut.ConsultarCarteiraAsync(1, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Exception.Should().NotBeNull();
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task ClienteService_Deve_RetornarException_Quando_ObterCotacoesFechamentoFalhar()
    {
        // Arrange
        var cliente = FakerRequest.ClienteAtivo().Generate();
        var cestaRequest = FakerRequest.CriarCestaRecomendadaRequest();
        var cestaAtiva = CestaRecomendada.CriarCesta(cestaRequest.Nome, cestaRequest.Itens.Select(x => ComposicaoCesta.CriaItemNaCesta(x.Ticker, x.Percentual)).ToList());

        _clienteRepository.ObterClienteAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())!
            .Returns(cliente);

        _cestaRecomendadaService.ObterCestaAtivaAsync(Arg.Any<CancellationToken>())
            .Returns(cestaAtiva);

        _cotacaoService.ObterCotacoesFechamentoB3DaCestaRecomendadaAsync(Arg.Any<CestaRecomendada>(), Arg.Any<CancellationToken>())!
            .Returns(new ApplicationException());

        // Act
        var result = await _sut.ConsultarCarteiraAsync(1, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Exception.Should().NotBeNull();
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task ClienteService_Deve_RetornarCarteira_Quando_NaoOcorrerFalhas()
    {
        // Arrange
        Cliente cliente = new(1, "Name", "11111111111", "email@teste.com", 3000, 3000, true, DateTime.MinValue);
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
        CarteiraCustodiaResponse resultadoEsperado = new(1,
            "Name", "",
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

        _clienteRepository.ObterClienteAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())!
            .Returns(cliente);

        _cestaRecomendadaService.ObterCestaAtivaAsync(Arg.Any<CancellationToken>())
            .Returns(cestaAtiva);

        _cotacaoService.ObterCotacoesFechamentoB3DaCestaRecomendadaAsync(Arg.Any<CestaRecomendada>(), Arg.Any<CancellationToken>())!
            .Returns(new Cotacao(1, DateTime.Now, new() { ComposicaoCotacao.CriarItem("PETR4", 36.50m), ComposicaoCotacao.CriarItem("VALE3", 68.20m), ComposicaoCotacao.CriarItem("ITUB4", 33.15m), ComposicaoCotacao.CriarItem("WEGE3", 42.05m), ComposicaoCotacao.CriarItem("MGLU3", 1.95m) }));

        // Act
        var result = await _sut.ConsultarCarteiraAsync(1, CancellationToken.None);

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
    public async Task ConsultarRentabilidade_Deve_RetornarClienteNaoEncontradoException_Quando_ClienteNaoExistir()
    {
        // Arrange
        _clienteRepository.ObterClienteAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())!
            .Returns((Cliente)null!);

        // Act
        Task result() => _sut.ConsultarRentabilidadeAsync(1, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<ClienteNaoEncontradoException>(result);
    }

    [Fact]
    public async Task ConsultarRentabilidade_Deve_RetornarException_Quando_CestaAtivaFalhar()
    {
        // Arrange
        var cliente = FakerRequest.ClienteAtivo().Generate();

        _clienteRepository.ObterClienteAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())!
            .Returns(cliente);

        _cestaRecomendadaService.ObterCestaAtivaAsync(Arg.Any<CancellationToken>())
            .Returns(new ApplicationException());

        // Act
        var result = await _sut.ConsultarRentabilidadeAsync(1, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Exception.Should().NotBeNull();
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task ConsultarRentabilidade_Deve_RetornarException_Quando_ObterCotacoesFechamentoFalhar()
    {
        // Arrange
        var cliente = FakerRequest.ClienteAtivo().Generate();
        var cestaRequest = FakerRequest.CriarCestaRecomendadaRequest();
        var cestaAtiva = CestaRecomendada.CriarCesta(cestaRequest.Nome, cestaRequest.Itens.Select(x => ComposicaoCesta.CriaItemNaCesta(x.Ticker, x.Percentual)).ToList());

        _clienteRepository.ObterClienteAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())!
            .Returns(cliente);

        _cestaRecomendadaService.ObterCestaAtivaAsync(Arg.Any<CancellationToken>())
            .Returns(cestaAtiva);

        _cotacaoService.ObterCotacoesFechamentoB3DaCestaRecomendadaAsync(Arg.Any<CestaRecomendada>(), Arg.Any<CancellationToken>())!
            .Returns(new ApplicationException());

        // Act
        var result = await _sut.ConsultarRentabilidadeAsync(1, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Exception.Should().NotBeNull();
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task ClienteService_Deve_RetornarRentabilidade_Quando_NaoOcorrerFalhas()
    {
        // Arrange
        Cliente cliente = new(1, "Name", "11111111111", "email@teste.com", 3000, 3000, true, DateTime.MinValue);
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
        RentabilidadeResponse resultadoEsperado = new(1, "Name", DateTime.Now,
            new ResumoCarteiraDto
            {
                ValorAtualCarteira = 27872,
                ValorTotalInvestido = 27754.50m,
                PlTotal = 117.50m,
                RentabilidadePercentual = 0.42m
            },
            new() { new HistoricoAporteDto { Parcela = "1/3", Valor = 1, Data = DateOnly.FromDateTime(new DateTime(2026, 03, 05)) } },
            new() { new EvolucaoCarteiraDto { Rentabilidade = 547400, ValorCarteira = 5475, ValorInvestido = 1, Data = DateOnly.FromDateTime(new DateTime(2026, 03, 05)) } });

        _clienteRepository.ObterClienteAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())!
            .Returns(cliente);

        _cestaRecomendadaService.ObterCestaAtivaAsync(Arg.Any<CancellationToken>())
            .Returns(cestaAtiva);

        _cotacaoService.ObterCotacoesFechamentoB3DaCestaRecomendadaAsync(Arg.Any<CestaRecomendada>(), Arg.Any<CancellationToken>())!
            .Returns(new Cotacao(1, DateTime.Now, new() { ComposicaoCotacao.CriarItem("PETR4", 36.50m), ComposicaoCotacao.CriarItem("VALE3", 68.20m), ComposicaoCotacao.CriarItem("ITUB4", 33.15m), ComposicaoCotacao.CriarItem("WEGE3", 42.05m), ComposicaoCotacao.CriarItem("MGLU3", 1.95m) }));

        // Act
        var result = await _sut.ConsultarRentabilidadeAsync(1, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Exception.Should().BeNull();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEquivalentTo(resultadoEsperado, options =>
            options.Using<DateTime>(ctx =>
                ctx.Subject.Date.Should().Be(ctx.Expectation.Date)
            ).WhenTypeIs<DateTime>());
    }
}