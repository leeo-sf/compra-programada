using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Interface;
using CompraProgramada.Application.Mapper;
using CompraProgramada.Application.Request;
using CompraProgramada.Application.Service;
using CompraProgramada.Application.Tests.TestUtils;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Exceptions;
using CompraProgramada.Domain.Interface;
using FluentAssertions;
using NSubstitute;
using OperationResult;

namespace CompraProgramada.Application.Tests.Service;

public class ClienteServiceTests
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IContaGraficaService _contaGraficaService;
    private readonly ICestaRecomendadaService _cestaRecomendadaService;
    private readonly ClienteMapper _mapper;
    private readonly ClienteService _sut;

    public ClienteServiceTests()
    {
        _clienteRepository = Substitute.For<IClienteRepository>();
        _contaGraficaService = Substitute.For<IContaGraficaService>();
        _cestaRecomendadaService = Substitute.For<ICestaRecomendadaService>();
        _mapper = Substitute.For<ClienteMapper>(Substitute.For<ContaMapper>());
        _sut = new(_clienteRepository, _contaGraficaService, _cestaRecomendadaService, _mapper);
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
        var command = FakerRequest.AdesaoRequest().Generate();
        var cestaRequest = FakerRequest.CriarCestaRecomendadaRequest();
        var cestaAtiva = CestaRecomendada.CriarCesta(cestaRequest.Nome, cestaRequest.Itens.Select(x => ComposicaoCesta.CriaItemNaCesta(x.Ticker, x.Percentual)).ToList());

        _clienteRepository.ExisteAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(false);
        
        _cestaRecomendadaService.ObterCestaAtivaAsync(Arg.Any<CancellationToken>())
            .Returns(cestaAtiva);

        _clienteRepository.CriarAsync(Arg.Any<Cliente>(), Arg.Any<CancellationToken>())
            .Returns(Cliente.Criar(command.Nome, command.Cpf, command.Email, command.ValorMensal));

        _contaGraficaService.GerarContaGraficaAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(ContaGrafica.Gerar(1));

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

    // Consultar Carteira

    // Consultar Rentabilidade
}