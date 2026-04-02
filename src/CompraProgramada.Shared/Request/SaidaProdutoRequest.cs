using CompraProgramada.Shared.Response;
using MediatR;
using OperationResult;

namespace CompraProgramada.Shared.Request;

public record SaidaProdutoRequest(int ClienteId) : IRequest<Result<SaidaProdutoResponse>>;