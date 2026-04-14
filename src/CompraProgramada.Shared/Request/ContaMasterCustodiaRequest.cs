using CompraProgramada.Shared.Response;
using MediatR;
using OperationResult;

namespace CompraProgramada.Shared.Request;

public record ContaMasterCustodiaRequest() : IRequest<Result<ContaMasterCustodiaResponse>>;