using Bogus;
using Bogus.Extensions.Brazil;
using CompraProgramada.Shared.Dto;
using CompraProgramada.Shared.Request;
using CompraProgramada.Domain.Entity;

namespace CompraProgramada.Application.Tests.TestUtils;

public static class FakerRequest
{
    public static Faker<AdesaoRequest> AdesaoRequest() => new Faker<AdesaoRequest>()
        .CustomInstantiator(f => new AdesaoRequest(
            Nome: f.Person.UserName,
            Cpf: f.Person.Cpf(false),
            Email: f.Person.Email,
            ValorMensal: f.Finance.Amount(100)
        ));

    public static Faker<List<Cliente>> ClientesAtivos() => new Faker<List<Cliente>>()
        .CustomInstantiator(f => new List<Cliente>
        {
            new Cliente(1, f.Person.UserName, f.Person.Cpf(false), f.Person.Email, 3000, 3000, true, DateTime.MinValue, new(1, "FLH-000001", DateTime.MinValue, new(1, f.Person.UserName, f.Person.Cpf(false), f.Person.Email, 3000, 3000, true, DateTime.MinValue), new() { }, new() { }, new() { })),
            new Cliente(2, f.Person.UserName, f.Person.Cpf(false), f.Person.Email, 6000, 6000, true, DateTime.MinValue, new(2, "FLH-000002", DateTime.MinValue, new(2, f.Person.UserName, f.Person.Cpf(false), f.Person.Email, 6000, 6000, true, DateTime.MinValue), new() { }, new() { }, new() { })),
            new Cliente(3, f.Person.UserName, f.Person.Cpf(false), f.Person.Email, 1500, 1500, true, DateTime.MinValue, new(3, "FLH-000003", DateTime.MinValue, new(3, f.Person.UserName, f.Person.Cpf(false), f.Person.Email, 1500, 1500, true, DateTime.MinValue), new() { }, new() { }, new() { })),
        });

    public static Faker<Cliente> ClienteAtivo() => new Faker<Cliente>()
        .CustomInstantiator(f => Cliente.Criar(new(f.Person.UserName, f.Person.Cpf(false), f.Person.Email, f.Finance.Amount(100))));

    public static Faker<AtualizarValorMensalRequest> AtualizarValorMensalRequest() => new Faker<AtualizarValorMensalRequest>()
        .CustomInstantiator(f => new AtualizarValorMensalRequest(
            ClienteId: f.IndexFaker + 1,
            NovoValorMensal: f.Finance.Amount(100)
        ));

    public static CriarCestaRecomendadaRequest CriarCestaRecomendadaRequest()
        => new CriarCestaRecomendadaRequest(
            Nome: "Cesta Top Five",
            Itens: ComposicaoCestaRecomendada());

    public static List<ComposicaoCestaDto> ComposicaoCestaRecomendada()
        => new List<ComposicaoCestaDto>
        {
            new ComposicaoCestaDto { Ticker = "PETR4", Percentual = 30 },
            new ComposicaoCestaDto { Ticker = "VALE3", Percentual = 25 },
            new ComposicaoCestaDto { Ticker = "ITUB4", Percentual = 20 },
            new ComposicaoCestaDto { Ticker = "BBDC4", Percentual = 15 },
            new ComposicaoCestaDto { Ticker = "WEGE3", Percentual = 10 }
        };

    public static List<OrdemCompra> OrdensCompraEmitidas()
        => new List<OrdemCompra>
        {
            OrdemCompra.GerarOrdemCompra("PETR4", 30, 35),
            OrdemCompra.GerarOrdemCompra("VALE3", 14, 62),
            OrdemCompra.GerarOrdemCompra("ITUB4", 23, 30),
            OrdemCompra.GerarOrdemCompra("BBDC4", 35, 15),
            OrdemCompra.GerarOrdemCompra("WEGE3", 8, 40)
        };

    public static List<Distribuicao> Distribuicoes()
    {
        var clientes = ClientesAtivos().Generate();
        var contaA = new ContaGrafica(1, "FLH-000001", DateTime.Now, clientes.First(x => x.Id == 1), new() { }, new() { }, new() { });
        var contaB = new ContaGrafica(2, "FLH-000002", DateTime.Now, clientes.First(x => x.Id == 2), new() { }, new() { }, new() { });
        var contaC = new ContaGrafica(3, "FLH-000003", DateTime.Now, clientes.First(x => x.Id == 3), new() { }, new() { }, new() { });

        return new List<Distribuicao>
        {
            Distribuicao.CriarDistribuicao(8, contaA, OrdemCompra.GerarOrdemCompra("PETR4", 30, 35)),
            Distribuicao.CriarDistribuicao(17, contaB, OrdemCompra.GerarOrdemCompra("PETR4", 30, 35)),
            Distribuicao.CriarDistribuicao(4, contaC, OrdemCompra.GerarOrdemCompra("PETR4", 30, 35)),
            Distribuicao.CriarDistribuicao(4, contaA, OrdemCompra.GerarOrdemCompra("VALE3", 14, 62)),
            Distribuicao.CriarDistribuicao(8, contaB, OrdemCompra.GerarOrdemCompra("VALE3", 14, 62)),
            Distribuicao.CriarDistribuicao(2, contaC, OrdemCompra.GerarOrdemCompra("VALE3", 14, 62)),
            Distribuicao.CriarDistribuicao(6, contaA, OrdemCompra.GerarOrdemCompra("ITUB4", 23, 30)),
            Distribuicao.CriarDistribuicao(13, contaB, OrdemCompra.GerarOrdemCompra("ITUB4", 23, 30)),
            Distribuicao.CriarDistribuicao(3, contaC, OrdemCompra.GerarOrdemCompra("ITUB4", 23, 30)),
            Distribuicao.CriarDistribuicao(10, contaA, OrdemCompra.GerarOrdemCompra("BBDC4", 35, 15)),
            Distribuicao.CriarDistribuicao(20, contaB, OrdemCompra.GerarOrdemCompra("BBDC4", 35, 15)),
            Distribuicao.CriarDistribuicao(5, contaC, OrdemCompra.GerarOrdemCompra("BBDC4", 35, 15)),
            Distribuicao.CriarDistribuicao(2, contaA, OrdemCompra.GerarOrdemCompra("WEGE3", 8, 40)),
            Distribuicao.CriarDistribuicao(4, contaB, OrdemCompra.GerarOrdemCompra("WEGE3", 8, 40)),
            Distribuicao.CriarDistribuicao(1, contaC, OrdemCompra.GerarOrdemCompra("WEGE3", 8, 40)),
        };
    }

    public static List<AtivoQuantidadeDto> ResiduosNaoDistribuidos()
        => new List<AtivoQuantidadeDto>
        {
            new AtivoQuantidadeDto { Ticker = "PETR4", Quantidade = 1 },
            new AtivoQuantidadeDto { Ticker = "VALE3", Quantidade = 0 },
            new AtivoQuantidadeDto { Ticker = "ITUB4", Quantidade = 1 },
            new AtivoQuantidadeDto { Ticker = "BBDC4", Quantidade = 0 },
            new AtivoQuantidadeDto { Ticker = "WEGE3", Quantidade = 1 },
        };
}