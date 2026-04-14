using CompraProgramada.Shared.Dto;
using CompraProgramada.Application.Mapper;
using CompraProgramada.Shared.Request;
using CompraProgramada.Shared.Response;
using MediatR;
using Microsoft.Extensions.Logging;
using OperationResult;
using CompraProgramada.Application.Contract.Service;

namespace CompraProgramada.Application.Handler;

public class AdministradorHandler
    : IRequestHandler<CestaAtualRequest, Result<CestaRecomendadaDto>>,
        IRequestHandler<CestaHistoricoRequest, Result<HistoricoCestasResponse>>,
        IRequestHandler<ContaMasterCustodiaRequest, Result<ContaMasterCustodiaResponse>>
{
    private readonly ILogger<AdministradorHandler> _logger;
    private readonly ICestaRecomendadaService _cestaService;
    private readonly CestaRecomendadaMapper _mapper;

    public AdministradorHandler(ILogger<AdministradorHandler> logger,
        ICestaRecomendadaService cestaService,
        CestaRecomendadaMapper mapper)
    {
        _logger = logger;
        _cestaService = cestaService;
        _mapper = mapper;
    }

    public async Task<Result<CestaRecomendadaDto>> Handle(CestaAtualRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando consulta da cesta atual.");

        var result = await _cestaService.ObterCestaAtivaAsync(cancellationToken);

        if (!result.IsSuccess)
            return result.Exception;

        // Obter fechamento atual para retornar

        var cesta = result.Value!;

        return _mapper.ToResponse(cesta);
    }

    public async Task<Result<HistoricoCestasResponse>> Handle(CestaHistoricoRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando processo de consulta de histórico de cestas.");

        var cestas = await _cestaService.HistoricoCestasAsync(cancellationToken);

        if (cestas.Value is null)
            return new HistoricoCestasResponse(new());

        var cestasDto = cestas.Value.Select(x => _mapper.ToResponse(x)).ToList();

        return new HistoricoCestasResponse(cestasDto);
    }

    public Task<Result<ContaMasterCustodiaResponse>> Handle(ContaMasterCustodiaRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}