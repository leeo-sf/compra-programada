using CompraProgramada.Domain.Contract.Repository;
using CompraProgramada.Domain.Contract.Service;
using CompraProgramada.Shared.Dto;
using CompraProgramada.Shared.Exceptions;
using CompraProgramada.Shared.Request;
using CompraProgramada.Shared.Response;
using MediatR;
using OperationResult;

namespace CompraProgramada.Domain.Handler.Api;

public class ConsultarCustodiaMasterHandler(IContaMasterRepository contaMasterRepository, ICotacaoService cotacaoService, ICestaRecomendadaRepository cestaRecomendadaRepository)
    : IRequestHandler<ConsultarCustodiaMasterRequest, Result<ConsultarCustodiaMasterResponse>>
{
    public async Task<Result<ConsultarCustodiaMasterResponse>> Handle(ConsultarCustodiaMasterRequest request, CancellationToken cancellationToken)
    {
        var contaMaster = await contaMasterRepository.ObterContaMasterAsync(cancellationToken);
        if (contaMaster is null)
            return new AppException("Conta master não encontrada", "CONTA_MASTER");

        var cesta = await cestaRecomendadaRepository.ObterCestaAtualAsync(cancellationToken);
        if (cesta is null)
            return new AppException("Nenhuma cesta recomendada encontrada", "CESTA_NAO_ENCONTRADA");

        var cotacaoResult = await cotacaoService.ObterCotacoesDaCestaRecomendadaAsync(cesta, cancellationToken);
        if (!cotacaoResult.IsSuccess)
            return cotacaoResult.Exception;

        var cotacao = cotacaoResult.Value;
        var custodias = contaMaster.CustodiaMasters.Where(c => c.QuantidadeResiduo > 0).ToList();

        return new ConsultarCustodiaMasterResponse(
            new ContaMasterDto
            {
                Id = contaMaster.Id,
                NumeroConta = contaMaster.NumeroConta,
                Tipo = contaMaster.Tipo,
            },
            [.. custodias.Select(c => {
                var precoFechamentoAtivo = cotacao.ComposicaoCotacao
                    .Where(cc => cc.Ticker == c.Ticker)
                    .Select(c => c.PrecoFechamento)
                    .FirstOrDefault();

                return new CustodiaMasterDto
                {
                    Ticker = c.Ticker,
                    Quantidade = c.QuantidadeResiduo,
                    PrecoMedio = c.PrecoMedio,
                    ValorAtual = precoFechamentoAtivo,
                    Origem = $"Residuo distribuicao {c.DataAtualizacao:yyyy-MM-dd}"
                };
            })]);
    }
}