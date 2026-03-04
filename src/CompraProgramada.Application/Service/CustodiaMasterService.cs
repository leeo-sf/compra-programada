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

        var custodias = residuos.Select(x => new CustodiaMaster(0, contaMaster.Id, x.Ticker, x.Quantidade)).ToList();

        await _contaRepository.AtualizarResiduosAysnc(contaMaster with { CustodiaMasters = custodias }, cancellationToken);

        return Result.Success();
    }

    public async Task<Result> AjustarResiduosAsync(List<AtivoDto> residuos, CancellationToken cancellationToken)
    {
        if (!residuos.Any())
            return new ApplicationException("Você precisa definir os resíduos.");

        var contaMaster = await _contaRepository.ObterContaMasterAsync(cancellationToken);

        if (contaMaster is null)
            return new ApplicationException("Conta master não encontrada!");

        var custodiasAtualizadas = contaMaster.CustodiaMasters
            .Select(cm => cm with { QuantidadeResiduo = Math.Max(0, residuos.First(x => x.Ticker == cm.Ticker).Quantidade) }).ToList();

        await _custodiaRepository.AtualizarResiduosAysnc(custodiasAtualizadas, cancellationToken);

        return Result.Success();
    }
}