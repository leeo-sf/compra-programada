using CompraProgramada.Application.Dto;
using MediatR;
using OperationResult;

namespace CompraProgramada.Application.Request;

public record CestaAtualRequest() : IRequest<Result<CestaRecomendadaDto>>;