using Bogus;
using Bogus.Extensions.Brazil;
using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Request;

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