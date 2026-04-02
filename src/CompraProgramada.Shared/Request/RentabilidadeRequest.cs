using CompraProgramada.Shared.Response;
using MediatR;
using OperationResult;

namespace CompraProgramada.Shared.Request;

public record RentabilidadeRequest(int ClienteId) : IRequest<Result<RentabilidadeResponse>>;