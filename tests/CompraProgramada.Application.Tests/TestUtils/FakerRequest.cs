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
        .CustomInstantiator(f =>
        {
            var cliente = Cliente.Criar(f.Person.UserName, f.Person.Cpf(false), f.Person.Email, f.Finance.Amount(100));
            cliente.AdicionarConta(ContaGrafica.Gerar(1));
            return cliente;
        });

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
}