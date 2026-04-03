using CompraProgramada.Domain.Entity;
using CompraProgramada.Shared.Dto;
using FluentAssertions;

namespace CompraProgramada.Domain.Tests.Entity;

public class ContaGraficaTests
{
    [Fact]
    public async Task Gerar_DeveRetornarContaGraficaComSucesso_Quando_DadosValidosInformados()
    {
        var cliente = Cliente.Criar(new("Name", "11111111111", "email@test.com", 150));
        var conta = ContaGrafica.Gerar(cliente);

        conta.Id.Should().Be(0);
        conta.NumeroConta.Should().Be("FLH-000000");
        conta.DataCriacao.Should().NotBeAfter(DateTime.Now);
        conta.Tipo.Should().Be("FILHOTE");
        conta.CustodiaFilhotes.Should().BeEmpty();
        conta.Distribuicoes.Should().BeEmpty();
        conta.HistoricoCompra.Should().BeEmpty();
    }

    [Fact]
    public async Task AdicionarCompra_DeveAdicionarComSucesso_Quando_Solicitado()
    {
        var cliente = Cliente.Criar(new("Name", "11111111111", "email@test.com", 150));
        var conta = ContaGrafica.Gerar(cliente);

        conta.AdicionarCompra(HistoricoCompra.RegistrarHistorico(1, "PETR4", 10, 10, 10, 50, DateOnly.FromDateTime(DateTime.Now)));

        conta.HistoricoCompra.Should().NotBeNull();
        conta.HistoricoCompra.Should().HaveCount(1);
    }

    [Fact]
    public async Task AdicionarDistribuicao_DeveAdicionarComSucesso_Quando_Solicitado()
    {
        var cliente = Cliente.Criar(new("Name", "11111111111", "email@test.com", 150));
        var conta = ContaGrafica.Gerar(cliente);
        var ordemCompra = OrdemCompra.GerarOrdemCompra("PETR4", 100, 42);

        conta.AdicionarDistribuicao(Distribuicao.CriarDistribuicao(10, conta, ordemCompra));

        conta.Distribuicoes.Should().NotBeNull();
        conta.Distribuicoes.Should().HaveCount(1);
    }

    [Fact]
    public async Task CalcularResumoDeRentabilidade_DeveRetornarApplicationException_Quando_CotacaoVazia()
    {
        ContaGrafica conta = new(1, "", DateTime.MinValue, Cliente.Criar(new("Name", "11111111111", "email@teste.com", 150)), new() { }, new() { }, new() { });
        Cotacao cotacao = new(1, DateTime.MinValue, new() { });

        var act = () => conta.CalcularResumoDeRentabilidade(cotacao) ;
        var exception = act.Should().Throw<ApplicationException>().Which;

        exception.Message.Should().Be("Itens do fechamento inválido!");
    }

    [Fact]
    public async Task CalcularResumoDeRentabilidade_DeveRetornarApplicationException_Quando_CustodiaFilhotesVazia()
    {
        ContaGrafica conta = new(1, "", DateTime.MinValue, Cliente.Criar(new("Name", "11111111111", "email@teste.com", 150)), new() { }, new() { }, new() { });
        Cotacao cotacao = new(1, DateTime.MinValue, new() { new(1, 1, "PETR4", 35) });

        var act = () => conta.CalcularResumoDeRentabilidade(cotacao) ;
        var exception = act.Should().Throw<ApplicationException>().Which;

        exception.Message.Should().Be("Cliente não tem uma carteira populada.");
    }

    [Fact]
    public async Task CalcularResumoDeRentabilidade_DeveRetornarResumo_Quando_DadosValidos()
    {
        Cliente cliente = Cliente.Criar(new("Name", "11111111111", "email@teste.com", 150));
        ContaGrafica conta = new(1, "", DateTime.MinValue, cliente, new() { }, new() { new(1, 1, "PETR4", 49.89m, 9), new(1, 1, "VALE3", 67.90m, 22) }, new() { });
        Cotacao cotacao = new(1, DateTime.MinValue, new() { new(1, 1, "PETR4", 35), new(1, 1, "VALE3", 65) });
        ResumoCarteiraDto resultadoEsperado = new ResumoCarteiraDto { ValorTotalInvestido = 1942.81m, ValorAtualCarteira = 1745, PlTotal = -197.81m, RentabilidadePercentual = -10.18m };

        var result = conta.CalcularResumoDeRentabilidade(cotacao);

        result.Should().BeEquivalentTo(resultadoEsperado);
    }

    [Fact]
    public async Task HistoricoAportes_DeveRetornarApplicationException_Quando_HistoricoCompraVazia()
    {
        ContaGrafica conta = new(1, "", DateTime.MinValue, Cliente.Criar(new("Name", "11111111111", "email@teste.com", 150)), new() { }, new() { }, new() { });

        var act = () => conta.HistoricoAportes();
        var exception = act.Should().Throw<ApplicationException>().Which;

        exception.Message.Should().Be("Cliente ainda não tem compras realizadas.");
    }

    [Theory]
    [MemberData(nameof(HistoricoAportes))]
    public async Task HistoricoAportes_DeveRetornarDetalhes_Quando_DadosValidos(List<HistoricoCompra> historicoCompras, List<HistoricoAporteDto> resultadoEsperado)
    {
        Cliente cliente = Cliente.Criar(new("Name", "11111111111", "email@teste.com", 150));
        ContaGrafica conta = new(1, "", DateTime.MinValue, cliente, new() { }, new() { }, historicoCompras);

        var result = conta.HistoricoAportes();

        result.Should().BeEquivalentTo(resultadoEsperado);
    }

    [Fact]
    public async Task CalcularEvolucaoCarteira_DeveRetornarApplicationException_Quando_CotacaoVazia()
    {
        ContaGrafica conta = new(1, "", DateTime.MinValue, Cliente.Criar(new("Name", "11111111111", "email@teste.com", 150)), new() { }, new() { }, new() { });
        Cotacao cotacao = new(1, DateTime.MinValue, new() { });

        var act = () => conta.CalcularEvolucaoCarteira(cotacao);
        var exception = act.Should().Throw<ApplicationException>().Which;

        exception.Message.Should().Be("Itens do fechamento inválido!");
    }

    [Fact]
    public async Task CalcularEvolucaoCarteira_DeveRetornarApplicationException_Quando_HistoricoCompraVazia()
    {
        ContaGrafica conta = new(1, "", DateTime.MinValue, Cliente.Criar(new("Name", "11111111111", "email@teste.com", 150)), new() { }, new() { }, new() { });
        Cotacao cotacao = new(1, DateTime.MinValue, new() { new(1, 1, "PETR4", 35) });

        var act = () => conta.CalcularEvolucaoCarteira(cotacao);
        var exception = act.Should().Throw<ApplicationException>().Which;

        exception.Message.Should().Be("Cliente ainda não tem compras realizadas.");
    }

    [Theory]
    [MemberData(nameof(EvolucaoCarteira))]
    public async Task CalcularEvolucaoCarteira_DeveRetornarEvolucao_Quando_DadosValidos(Cotacao cotacao, List<HistoricoCompra> historicoCompras, List<EvolucaoCarteiraDto> resultadoEsperado)
    {
        Cliente cliente = Cliente.Criar(new("Name", "11111111111", "email@teste.com", 3000));
        ContaGrafica conta = new(1, "", DateTime.MinValue, cliente, new() { }, new() { }, historicoCompras);

        var result = conta.CalcularEvolucaoCarteira(cotacao);

        result.Should().BeEquivalentTo(resultadoEsperado);
    }

    [Fact]
    public async Task CalcularDetalhesCarteira_DeveRetornarApplicationException_Quando_CotacaoVazia()
    {
        ContaGrafica conta = new(1, "", DateTime.MinValue, Cliente.Criar(new("Name", "11111111111", "email@teste.com", 150)), new() { }, new() { }, new() { });
        Cotacao cotacao = new(1, DateTime.MinValue, new() { });

        var act = () => conta.CalcularDetalhesCarteira(cotacao, 1000);
        var exception = act.Should().Throw<ApplicationException>().Which;

        exception.Message.Should().Be("Itens do fechamento inválido!");
    }

    [Fact]
    public async Task CalcularDetalhesCarteira_DeveRetornarApplicationException_Quando_CustodiaFilhotesVazia()
    {
        ContaGrafica conta = new(1, "", DateTime.MinValue, Cliente.Criar(new("Name", "11111111111", "email@teste.com", 150)), new() { }, new() { }, new() { });
        Cotacao cotacao = new(1, DateTime.MinValue, new() { new(1, 1, "PETR4", 35) });

        var act = () => conta.CalcularDetalhesCarteira(cotacao, 1000);
        var exception = act.Should().Throw<ApplicationException>().Which;

        exception.Message.Should().Be("Conta não tem uma carteira no momento.");
    }

    [Theory]
    [MemberData(nameof(DetalhesCarteira))]
    public async Task CalcularDetalhesCarteira_DeveRetornarDetalhes_Quando_DadosValidos(decimal valorAtualCarteira, Cotacao cotacao, List<DetalheCarteiraDto> resultadoEsperado)
    {
        Cliente cliente = Cliente.Criar(new("Name", "11111111111", "email@teste.com", 3000));
        ContaGrafica conta = new(1, "", DateTime.MinValue, cliente, new() { }, new() { new(1, 1, "PETR4", 35.50m, 24), new(1, 1, "VALE3", 60, 12), new(1, 1, "ITUB4", 29, 18), new(1, 1, "BBDC4", 14.50m, 30), new(1, 1, "WEGE3", 38, 6) }, new() { });

        var result = conta.CalcularDetalhesCarteira(cotacao, valorAtualCarteira);

        result.Should().BeEquivalentTo(resultadoEsperado);
    }

    public static TheoryData<List<HistoricoCompra>, List<HistoricoAporteDto>> HistoricoAportes()
    {
        return new()
        {
            {
                new List<HistoricoCompra>
                {
                    new(1, 1, "PETR4", 12, 35, 35, 1000, DateOnly.FromDateTime(new DateTime(2026, 03, 05)))
                },
                new List<HistoricoAporteDto>
                {
                    new() { Data = DateOnly.FromDateTime(new DateTime(2026, 03, 05)), Valor = 1000, Parcela = "1/3" }
                }
            },
            {
                new List<HistoricoCompra>
                {
                    new(1, 1, "PETR4", 12, 35, 35, 1000, DateOnly.FromDateTime(new DateTime(2026, 03, 05))),
                    new(1, 1, "VALE3", 17, 65, 65, 1000, DateOnly.FromDateTime(new DateTime(2026, 03, 05)))
                },
                new List<HistoricoAporteDto>
                {
                    new() { Data = DateOnly.FromDateTime(new DateTime(2026, 03, 05)), Valor = 1000, Parcela = "1/3" }
                }
            },
            {
                new List<HistoricoCompra>
                {
                    new(1, 1, "PETR4", 12, 35, 35, 1000, DateOnly.FromDateTime(new DateTime(2026, 03, 05))),
                    new(1, 1, "VALE3", 17, 65, 65, 1000, DateOnly.FromDateTime(new DateTime(2026, 03, 05))),
                    new(1, 1, "ITUB4", 77, 55.78m, 90, 3000, DateOnly.FromDateTime(new DateTime(2026, 03, 15))),
                    new(1, 1, "AAPL4", 23, 92.21m, 77, 3000, DateOnly.FromDateTime(new DateTime(2026, 03, 15)))
                },
                new List<HistoricoAporteDto>
                {
                    new() { Data = DateOnly.FromDateTime(new DateTime(2026, 03, 05)), Valor = 1000, Parcela = "1/3" },
                    new() { Data = DateOnly.FromDateTime(new DateTime(2026, 03, 15)), Valor = 3000, Parcela = "2/3" }
                }
            },
            {
                new List<HistoricoCompra>
                {
                    new(1, 1, "PETR4", 12, 35, 35, 1000, DateOnly.FromDateTime(new DateTime(2026, 03, 05))),
                    new(1, 1, "ITUB4", 77, 55.78m, 90, 7000, DateOnly.FromDateTime(new DateTime(2026, 03, 15))),
                    new(1, 1, "AAPL4", 23, 92.21m, 77, 3000, DateOnly.FromDateTime(new DateTime(2026, 04, 05)))
                },
                new List<HistoricoAporteDto>
                {
                    new() { Data = DateOnly.FromDateTime(new DateTime(2026, 03, 05)), Valor = 1000, Parcela = "1/3" },
                    new() { Data = DateOnly.FromDateTime(new DateTime(2026, 03, 15)), Valor = 7000, Parcela = "2/3" },
                    new() { Data = DateOnly.FromDateTime(new DateTime(2026, 04, 05)), Valor = 3000, Parcela = "1/3" }
                }
            }
        };
    }

    public static TheoryData<Cotacao, List<HistoricoCompra>, List<EvolucaoCarteiraDto>> EvolucaoCarteira()
    {
        return new()
        {
            {
                new Cotacao(1, DateTime.MinValue, new() { new(1, 1, "AAPL4", 28) }),
                new List<HistoricoCompra>
                {
                    new(1, 1, "PETR4", 5, 35, 35, 5000, DateOnly.FromDateTime(new DateTime(2026, 03, 05)))
                },
                new List<EvolucaoCarteiraDto>
                {
                    new EvolucaoCarteiraDto { Data = DateOnly.FromDateTime(new DateTime(2026, 03, 05)), Rentabilidade = 0, ValorCarteira = 0, ValorInvestido = 5000 }
                }
            },
            {
                new Cotacao(1, DateTime.MinValue, new() { new(1, 1, "PETR4", 28), new(1, 1, "VALE3", 57) }),
                new List<HistoricoCompra>
                {
                    new(1, 1, "PETR4", 5, 35, 35, 5000, DateOnly.FromDateTime(new DateTime(2026, 03, 05))),
                    new(1, 1, "VALE3", 9, 65, 65, 5000, DateOnly.FromDateTime(new DateTime(2026, 03, 05)))
                },
                new List<EvolucaoCarteiraDto>
                {
                    new EvolucaoCarteiraDto { Data = DateOnly.FromDateTime(new DateTime(2026, 03, 05)), Rentabilidade = -86.94m, ValorCarteira = 653, ValorInvestido = 5000 }
                }
            },
            {
                new Cotacao(1, DateTime.MinValue, new() { new(1, 1, "PETR4", 39), new(1, 1, "VALE3", 62), new(1, 1, "ITUB4", 31), new(1, 1, "BBDC4", 16), new(1, 1, "WEGE3", 41.50m) }),
                new List<HistoricoCompra>
                {
                    new(1, 1, "PETR4", 12, 35, 35, 1000, DateOnly.FromDateTime(new DateTime(2026, 03, 05))),
                    new(1, 1, "VALE3", 17, 62, 62, 1000, DateOnly.FromDateTime(new DateTime(2026, 03, 05))),
                    new(1, 1, "ITUB4", 11, 30, 30, 1000, DateOnly.FromDateTime(new DateTime(2026, 03, 05))),
                    new(1, 1, "BBDC4", 19, 15, 15, 1000, DateOnly.FromDateTime(new DateTime(2026, 03, 05))),
                    new(1, 1, "WEGE3", 26, 40, 40, 1000, DateOnly.FromDateTime(new DateTime(2026, 03, 05))),

                    new(1, 1, "PETR4", 8, 39, 35, 1000, DateOnly.FromDateTime(new DateTime(2026, 03, 15))),
                    new(1, 1, "VALE3", 5, 60, 62, 1000, DateOnly.FromDateTime(new DateTime(2026, 03, 15))),
                    new(1, 1, "ITUB4", 3, 33, 30, 1000, DateOnly.FromDateTime(new DateTime(2026, 03, 15))),
                    new(1, 1, "BBDC4", 1, 17, 15, 1000, DateOnly.FromDateTime(new DateTime(2026, 03, 15))),
                    new(1, 1, "WEGE3", 2, 44, 40, 1000, DateOnly.FromDateTime(new DateTime(2026, 03, 15)))
                },
                new List<EvolucaoCarteiraDto>
                {
                    new EvolucaoCarteiraDto { Data = DateOnly.FromDateTime(new DateTime(2026, 03, 05)), Rentabilidade = 224.60m, ValorCarteira = 3246, ValorInvestido = 1000 },
                    new EvolucaoCarteiraDto { Data = DateOnly.FromDateTime(new DateTime(2026, 03, 15)), Rentabilidade = -59.30m, ValorCarteira = 814, ValorInvestido = 2000 }
                }
            },
            {
                new Cotacao(1, DateTime.MinValue, new() { new(1, 1, "PETR4", 39), new(1, 1, "VALE3", 62), new(1, 1, "ITUB4", 31), new(1, 1, "BBDC4", 16), new(1, 1, "WEGE3", 41.50m) }),
                new List<HistoricoCompra>
                {
                    new(1, 1, "PETR4", 12, 35, 35, 1000, DateOnly.FromDateTime(new DateTime(2026, 03, 05))),
                    new(1, 1, "VALE3", 17, 62, 62, 1000, DateOnly.FromDateTime(new DateTime(2026, 03, 05))),
                    new(1, 1, "ITUB4", 11, 30, 30, 1000, DateOnly.FromDateTime(new DateTime(2026, 03, 05))),
                    new(1, 1, "BBDC4", 19, 15, 15, 1000, DateOnly.FromDateTime(new DateTime(2026, 03, 05))),
                    new(1, 1, "WEGE3", 26, 40, 40, 1000, DateOnly.FromDateTime(new DateTime(2026, 03, 05))),

                    new(1, 1, "PETR4", 8, 39, 35, 1000, DateOnly.FromDateTime(new DateTime(2026, 03, 15))),
                    new(1, 1, "VALE3", 5, 60, 62, 1000, DateOnly.FromDateTime(new DateTime(2026, 03, 15))),
                    new(1, 1, "ITUB4", 3, 33, 30, 1000, DateOnly.FromDateTime(new DateTime(2026, 03, 15))),
                    new(1, 1, "BBDC4", 1, 17, 15, 1000, DateOnly.FromDateTime(new DateTime(2026, 03, 15))),
                    new(1, 1, "WEGE3", 2, 44, 40, 1000, DateOnly.FromDateTime(new DateTime(2026, 03, 15))),

                    new(1, 1, "PETR4", 10, 39, 35, 3350, DateOnly.FromDateTime(new DateTime(2026, 03, 25))),
                    new(1, 1, "VALE3", 22, 60, 62, 3350, DateOnly.FromDateTime(new DateTime(2026, 03, 25))),
                    new(1, 1, "ITUB4", 33, 33, 30, 3350, DateOnly.FromDateTime(new DateTime(2026, 03, 25))),
                    new(1, 1, "BBDC4", 37, 17, 15, 3350, DateOnly.FromDateTime(new DateTime(2026, 03, 25))),
                    new(1, 1, "WEGE3", 21, 44, 40, 3350, DateOnly.FromDateTime(new DateTime(2026, 03, 25)))
                },
                new List<EvolucaoCarteiraDto>
                {
                    new EvolucaoCarteiraDto { Data = DateOnly.FromDateTime(new DateTime(2026, 03, 05)), Rentabilidade = 224.60m, ValorCarteira = 3246, ValorInvestido = 1000 },
                    new EvolucaoCarteiraDto { Data = DateOnly.FromDateTime(new DateTime(2026, 03, 15)), Rentabilidade = -59.30m, ValorCarteira = 814, ValorInvestido = 2000 },
                    new EvolucaoCarteiraDto { Data = DateOnly.FromDateTime(new DateTime(2026, 03, 25)), Rentabilidade = -20.74m, ValorCarteira = 4240.50m, ValorInvestido = 5350 }
                }
            }
        };
    }

    public static TheoryData<decimal, Cotacao, List<DetalheCarteiraDto>> DetalhesCarteira()
        => new()
        {
            {
                6450,
                new(1, DateTime.MinValue, new() { new(1, 1, "PETR4", 37), new(1, 1, "VALE3", 65), new(1, 1, "ITUB4", 31), new(1, 1, "BBDC4", 15.50m), new(1, 1, "WEGE3", 42) }),
                new()
                {
                    new() { Ticker = "PETR4", Quantidade = 24, PrecoMedio = 35.50m, CotacaoAtual = 37, ValorAtual = 888, Pl = 36, PlPercentual = 4.23m, ComposicaoCarteira = 13.77m },
                    new() { Ticker = "VALE3", Quantidade = 12, PrecoMedio = 60, CotacaoAtual = 65, ValorAtual = 780, Pl = 60, PlPercentual = 8.33m, ComposicaoCarteira = 12.09m },
                    new() { Ticker = "ITUB4", Quantidade = 18, PrecoMedio = 29, CotacaoAtual = 31, ValorAtual = 558, Pl = 36, PlPercentual = 6.90m, ComposicaoCarteira = 8.65m },
                    new() { Ticker = "BBDC4", Quantidade = 30, PrecoMedio = 14.50m, CotacaoAtual = 15.50m, ValorAtual = 465, Pl = 30, PlPercentual = 6.90m, ComposicaoCarteira = 7.21m },
                    new() { Ticker = "WEGE3", Quantidade = 6, PrecoMedio = 38, CotacaoAtual = 42, ValorAtual = 252, Pl = 24, PlPercentual = 10.53m, ComposicaoCarteira = 3.91m }
                }
            },
            {
                1000,
                new(1, DateTime.MinValue, new() { new(1, 1, "AAPL4", 37) }),
                new()
                {
                    new() { Ticker = "PETR4", Quantidade = 24, PrecoMedio = 35.50m, CotacaoAtual = 0, ValorAtual = 0, Pl = -852, PlPercentual = 0, ComposicaoCarteira = 0 },
                    new() { Ticker = "VALE3", Quantidade = 12, PrecoMedio = 60, CotacaoAtual = 0, ValorAtual = 0, Pl = -720, PlPercentual = 0, ComposicaoCarteira = 0 },
                    new() { Ticker = "ITUB4", Quantidade = 18, PrecoMedio = 29, CotacaoAtual = 0, ValorAtual = 0, Pl = -522, PlPercentual = 0, ComposicaoCarteira = 0 },
                    new() { Ticker = "BBDC4", Quantidade = 30, PrecoMedio = 14.50m, CotacaoAtual = 0, ValorAtual = 0, Pl = -435, PlPercentual = 0, ComposicaoCarteira = 0 },
                    new() { Ticker = "WEGE3", Quantidade = 6, PrecoMedio = 38, CotacaoAtual = 0, ValorAtual = 0, Pl = -228, PlPercentual = 0, ComposicaoCarteira = 0 }
                }
            }
        };
}