using Bogus;
using Bogus.Extensions.Brazil;
using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Request;
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
            Cliente.Criar(f.Person.UserName, f.Person.Cpf(false), f.Person.Email, f.Finance.Amount(100)),
            Cliente.Criar(f.Person.UserName, f.Person.Cpf(false), f.Person.Email, f.Finance.Amount(150)),
            Cliente.Criar(f.Person.UserName, f.Person.Cpf(false), f.Person.Email, f.Finance.Amount(10000))
        });

    public static Faker<Cliente> ClienteAtivo() => new Faker<Cliente>()
        .CustomInstantiator(f => Cliente.Criar(f.Person.UserName, f.Person.Cpf(false), f.Person.Email, f.Finance.Amount(100)));

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
            new("PETR4", 30),
            new("VALE3", 25),
            new("ITUB4", 20),
            new("BBDC4", 15),
            new("WEGE3", 10)
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
        var contaA = ContaGrafica.Gerar(ClienteAtivo().Generate());
        var contaB = ContaGrafica.Gerar(ClienteAtivo().Generate());
        var contaC = ContaGrafica.Gerar(ClienteAtivo().Generate());

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

    public static List<AtivoDto> ResiduosNaoDistribuidos()
        => new List<AtivoDto>
        {
            new AtivoDto("PETR4", 1),
            new AtivoDto("VALE3", 0),
            new AtivoDto("ITUB4", 1),
            new AtivoDto("BBDC4", 0),
            new AtivoDto("WEGE3", 1),
        };
}