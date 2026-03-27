using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Interface;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;
using OperationResult;

namespace CompraProgramada.Application.Service;

public class CustodiaMasterService : ICustodiaMasterService
{
    private readonly ICustodiaMasterRepository _custodiaRepository;

    public CustodiaMasterService(ICustodiaMasterRepository custodiaRepository) => _custodiaRepository = custodiaRepository;

    public async Task<Result<List<CustodiaMaster>?>> ObterResiduosNaoDistribuidos(CancellationToken cancellationToken)
    {
        var result = await _custodiaRepository.ObterResiduosAsync(cancellationToken);

        return result;
    }

    public async Task<Result> AtualizarResiduosAsync(List<CustodiaMaster> custodias, CancellationToken cancellationToken)
    {
        if (!custodias.Any())
            return new ApplicationException("Um grupo para compra deve ser informado.");

        await _custodiaRepository.AtualizarResiduosAysnc(custodias, cancellationToken);

        return Result.Success();
    }

    public async Task<Result<List<AtivoDto>>> CapturarResiduosNaoDistribuidosAsync(List<Distribuicao> distribuicoes, List<OrdemCompra> ordensCompra, CancellationToken cancellationToken)
    {
        var custodias = await _custodiaRepository.ObterResiduosAsync(cancellationToken);

        foreach (var ativo in ordensCompra)
        {
            var custodia = custodias?.FirstOrDefault(x => x.Ticker == ativo.Ticker);

            if (custodia is null)
            {
                var novaCustodia = CustodiaMaster.CriarCustodia(1, ativo.Ticker);
                custodia = novaCustodia;
                custodias?.Add(custodia);
            }

            var qtdUtilizada = distribuicoes.Where(x => x.Ticker == ativo.Ticker)
                .Sum(x => x.QuantidadeAlocada);

            custodia?.AtualizarResiduo(ativo.QuantidadeTotal, qtdUtilizada);
        }

        await AtualizarResiduosAsync(custodias!, cancellationToken);

        return custodias!.Select(x => new AtivoDto(x.Ticker, x.QuantidadeResiduo)).ToList();
    }
}