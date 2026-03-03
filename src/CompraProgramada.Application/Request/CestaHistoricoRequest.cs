using CompraProgramada.Application.Response;
using MediatR;

namespace CompraProgramada.Application.Request;

public record CestaHistoricoRequest() : IRequest<Result<List<CestaResponse>>>;