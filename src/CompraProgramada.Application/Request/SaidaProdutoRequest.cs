using CompraProgramada.Application.Response;
using MediatR;
using OperationResult;

namespace CompraProgramada.Application.Request;

public record SaidaProdutoRequest(int ClienteId) : IRequest<Result<SaidaProdutoResponse>>;