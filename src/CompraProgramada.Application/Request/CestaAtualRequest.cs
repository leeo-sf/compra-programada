using CompraProgramada.Application.Response;
using MediatR;

namespace CompraProgramada.Application.Request;

public record CestaAtualRequest() : IRequest<Result<CestaResponse>>;