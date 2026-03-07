using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Interface;
using OperationResult;

namespace CompraProgramada.Application.Service;

public class PrecoMedioService : IPrecoMedioService
{
    public Result<decimal> CalcularPrecoMedio(PrecoMedioDto custodia)
    {
        if (custodia.QuantidadeNovosAtivos == 0)
            return 0;

        decimal precoMedio = 0;
        var valorCompraAnterior = custodia.QuantidadeAtivosAnterior * custodia.PrecoMedioAnterior;
        var valorCompraAtual = custodia.QuantidadeNovosAtivos * custodia.PrecoFechamentoAtivo;

        if (custodia.QuantidadeAtivosAnterior > 0)
            precoMedio = (valorCompraAnterior + valorCompraAtual) / custodia.QuantidadeAtivosAnterior + custodia.QuantidadeNovosAtivos;
        else
            precoMedio = valorCompraAtual / custodia.QuantidadeNovosAtivos;

        return precoMedio;
    }
}