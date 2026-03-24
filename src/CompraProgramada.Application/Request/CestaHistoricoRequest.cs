using CompraProgramada.Application.Response;
using MediatR;
using OperationResult;

namespace CompraProgramada.Application.Request;

public record CestaHistoricoRequest() : IRequest<Result<HistoricoCestasResponse>>;