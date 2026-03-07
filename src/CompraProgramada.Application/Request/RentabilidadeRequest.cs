using CompraProgramada.Application.Response;
using MediatR;
using OperationResult;

namespace CompraProgramada.Application.Request;

public record RentabilidadeRequest(int ClienteId) : IRequest<Result<RentabilidadeResponse>>;