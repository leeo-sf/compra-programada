using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Interface;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;

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

    public async Task<Result> RegistrarResiduosAsync(List<AtivoDto> residuos, CancellationToken cancellationToken)
    {
        if (!residuos.Any())
            return Result.Fail(new ApplicationException("Você precisa definir os resíduos."));

        var contaMaster = await _contaRepository.ObterContaMasterAsync(cancellationToken);

        if (contaMaster is null)
            return Result.Fail(new ApplicationException("Conta master não encontrada!"));

        var custodias = residuos.Select(x => new CustodiaMaster(0, contaMaster.Id, x.Ticker, x.Quantidade)).ToList();

        await _contaRepository.AtualizarResiduosAysnc(contaMaster with { CustodiaMasters = custodias }, cancellationToken);

        return Result.Ok();
    }
}