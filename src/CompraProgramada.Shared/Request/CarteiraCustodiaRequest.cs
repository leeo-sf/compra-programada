using CompraProgramada.Shared.Response;
using MediatR;
using OperationResult;

namespace CompraProgramada.Shared.Request;

public record CarteiraCustodiaRequest(int ClienteId) : IRequest<Result<CarteiraCustodiaResponse>>;