using CompraProgramada.Shared.Response;
using MediatR;
using OperationResult;

namespace CompraProgramada.Shared.Request;

public record ConsultarCustodiaMasterRequest() : IRequest<Result<ConsultarCustodiaMasterResponse>>;