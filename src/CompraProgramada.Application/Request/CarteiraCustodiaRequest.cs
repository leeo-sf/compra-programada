using CompraProgramada.Application.Response;
using MediatR;
using OperationResult;

namespace CompraProgramada.Application.Request;

public record CarteiraCustodiaRequest(int ClienteId) : IRequest<Result<CarteiraCustodiaResponse>>;