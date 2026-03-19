using CompraProgramada.Domain.Entity;

namespace CompraProgramada.Domain.Tests.TestsUtils;

public static class CestaRecomendadaFaker
{
    public static IEnumerable<object[]> CestaRecomendadaDadosValidos()
    {
        yield return new object[]
        {
            CestaRecomendada.CriarCesta("Cesta A", ComposicaoCestaValida())
        };
    }

    public static IEnumerable<object[]> CestaRecomendadaInativa()
    {
        var cesta = CestaRecomendada.CriarCesta("Cesta A", ComposicaoCestaValida());
        cesta.DesativarCesta();

        yield return new object[] { cesta };
    }

    public static IEnumerable<object[]> ComposicaoCestaDadosValidos()
    {
        yield return new object[]
        {
            ComposicaoCestaValida()
        };
    }

    public static IEnumerable<object[]> ComposicaoCestaQtdInvalida()
    {
        yield return new object[]
        {
            new List<ComposicaoCesta>
            {
                ComposicaoCesta.CriaItemNaCesta("PETR4", 30),
                ComposicaoCesta.CriaItemNaCesta("VALE3", 25),
                ComposicaoCesta.CriaItemNaCesta("ITUB4", 20),
                ComposicaoCesta.CriaItemNaCesta("BBDC4", 15)
            }
        };

        yield return new object[]
        {
            new List<ComposicaoCesta>
            {
                ComposicaoCesta.CriaItemNaCesta("PETR4", 30),
                ComposicaoCesta.CriaItemNaCesta("VALE3", 25)
            }
        };
    }

    public static IEnumerable<object[]> ComposicaoCestaPercentualInvalida()
    {
        yield return new object[]
        {
            new List<ComposicaoCesta>
            {
                ComposicaoCesta.CriaItemNaCesta("PETR4", 30),
                ComposicaoCesta.CriaItemNaCesta("VALE3", 25),
                ComposicaoCesta.CriaItemNaCesta("ITUB4", 20),
                ComposicaoCesta.CriaItemNaCesta("BBDC4", 15),
                ComposicaoCesta.CriaItemNaCesta("WEGE3", 15)
            }
        };

        yield return new object[]
        {
            new List<ComposicaoCesta>
            {
                ComposicaoCesta.CriaItemNaCesta("PETR4", 30),
                ComposicaoCesta.CriaItemNaCesta("VALE3", 25),
                ComposicaoCesta.CriaItemNaCesta("ITUB4", 25),
                ComposicaoCesta.CriaItemNaCesta("BBDC4", 15),
                ComposicaoCesta.CriaItemNaCesta("WEGE3", 10)
            }
        };

        yield return new object[]
        {
            new List<ComposicaoCesta>
            {
                ComposicaoCesta.CriaItemNaCesta("PETR4", 30),
                ComposicaoCesta.CriaItemNaCesta("VALE3", 25),
                ComposicaoCesta.CriaItemNaCesta("ITUB4", 20),
                ComposicaoCesta.CriaItemNaCesta("BBDC4", 15),
                ComposicaoCesta.CriaItemNaCesta("WEGE3", 5)
            }
        };
    }

    private static List<ComposicaoCesta> ComposicaoCestaValida()
        => new List<ComposicaoCesta>
        {
            ComposicaoCesta.CriaItemNaCesta("PETR4", 30),
            ComposicaoCesta.CriaItemNaCesta("VALE3", 25),
            ComposicaoCesta.CriaItemNaCesta("ITUB4", 20),
            ComposicaoCesta.CriaItemNaCesta("BBDC4", 15),
            ComposicaoCesta.CriaItemNaCesta("WEGE3", 10)
        };
}