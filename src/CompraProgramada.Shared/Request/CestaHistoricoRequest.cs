using CompraProgramada.Shared.Response;
using MediatR;
using OperationResult;

namespace CompraProgramada.Shared.Request;

public record CestaHistoricoRequest() : IRequest<Result<HistoricoCestasResponse>>;