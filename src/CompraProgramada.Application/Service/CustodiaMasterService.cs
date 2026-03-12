using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Interface;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;
using OperationResult;

namespace CompraProgramada.Application.Service;

public class CustodiaMasterService : ICustodiaMasterService
{
    private readonly IContaMasterRepository _contaRepository;
    private readonly ICustodiaMasterRepository _custodiaRepository;

    public CustodiaMasterService(ICustodiaMasterRepository custodiaRepository,
        IContaMasterRepository contaRepository)
    {
        _custodiaRepository = custodiaRepository;
        _contaRepository = contaRepository;
    }

    public async Task<Result<List<CustodiaMasterDto>?>> ObterResiduosNaoDistribuidos(CancellationToken cancellationToken)
    {
        var result = await _custodiaRepository.ObterResiduosAsync(cancellationToken);

        return result?.Select(c => new CustodiaMasterDto
        {
            Id = c.Id,
            ContaMasterId = c.ContaMasterId,
            Ticker = c.Ticker,
            QuantidadeResiduos = c.QuantidadeResiduo
        }).ToList();
    }

    public async Task<Result> RegistrarResiduosAsync(List<AtivoDto> residuos, CancellationToken cancellationToken)
    {
        if (!residuos.Any())
            return new ApplicationException("Você precisa definir os resíduos.");

        var contaMaster = await _contaRepository.ObterContaMasterAsync(cancellationToken);

        if (contaMaster is null)
            return new ApplicationException("Conta master não encontrada!");

        var custodias = residuos.Select(x => CustodiaMaster.CriarCustodia(contaMaster.Id, x.Ticker, x.Quantidade)).ToList();

        contaMaster.AtualizaCustodia(custodias);

        await _contaRepository.AtualizarResiduosAysnc(contaMaster, cancellationToken);

        return Result.Success();
    }

    public async Task<Result> AtualizarResiduosAsync(List<ResiduoCustodiaMasterDto> residuos, CancellationToken cancellationToken)
    {
        if (!residuos.Any())
            return new ApplicationException("Um grupo para compra deve ser informado.");

        var custodias = await _custodiaRepository.ObterResiduosAsync(cancellationToken);

        if (custodias is null)
            return new ApplicationException("Conta master não encontrada!");

        if (!custodias.Any())
        {
            var result = await RegistrarResiduosAsync(residuos.Select(x => new AtivoDto(x.Ticker, x.Quantidade)).ToList(), cancellationToken);
            if (!result.IsSuccess)
                return result.Exception;

            return Result.Success();
        }

        foreach (var custodia in custodias)
        {
            var ativo = residuos.FirstOrDefault(a => a.Ticker == custodia.Ticker);
            if (ativo is null)
                continue;

            custodia.AtualizarResiduo(ativo.Quantidade);
        }

        await _custodiaRepository.AtualizarResiduosAysnc(custodias, cancellationToken);

        return Result.Success();
    }

    public async Task<Result<List<ResiduoCustodiaMasterDto>>> CapturarResiduosDeCustodiaDistribuida(List<AtivoAhCompraDto> grupoAhDistribuir, List<DistribuicaoDto> distribuicaoRealizada, CancellationToken cancellationToken)
    {
        if (!grupoAhDistribuir.Any() || !distribuicaoRealizada.Any())
            return new ApplicationException("Para a captura de resíduos ser realizada precisa ser informado os grupos de distribuição");

        var custodiasParaAtualizar = new List<ResiduoCustodiaMasterDto>();

        foreach (var grupo in grupoAhDistribuir)
        {
            var quantidadeDistribuida = distribuicaoRealizada.Where(x => x.Ticker == grupo.Ticker).Sum(x => x.QuantidadeAlocada);

            var residuo = Math.Abs(quantidadeDistribuida - grupo.Quantidade);

            custodiasParaAtualizar.Add(new ResiduoCustodiaMasterDto(grupo.Ticker, residuo));
        }

        var result = await AtualizarResiduosAsync(custodiasParaAtualizar, cancellationToken);
        if (!result.IsSuccess)
            return result.Exception;

        return custodiasParaAtualizar;
    }
    
    public int SubtrairResiduosParaCompra(CustodiaMasterDto? custodia, int quantidadeCompraAtivo)
    {
        var residuoAtual = custodia?.QuantidadeResiduos ?? 0;

        if (residuoAtual == 0)
            return quantidadeCompraAtivo;

        var quantidadeResiduos = Math.Abs(residuoAtual - quantidadeCompraAtivo);
        var novaQuantidadeCompraAtivos = quantidadeCompraAtivo - residuoAtual;

        var necessidadeLiquida = quantidadeCompraAtivo - residuoAtual;
        if (necessidadeLiquida < 0)
            novaQuantidadeCompraAtivos = residuoAtual - quantidadeCompraAtivo;

        return novaQuantidadeCompraAtivos;
    }
}