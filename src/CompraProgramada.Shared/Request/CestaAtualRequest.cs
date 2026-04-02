using CompraProgramada.Shared.Dto;
using MediatR;
using OperationResult;

namespace CompraProgramada.Shared.Request;

public record CestaAtualRequest() : IRequest<Result<CestaRecomendadaDto>>;