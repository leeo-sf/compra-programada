using CompraProgramada.Shared.Dto;
using CompraProgramada.Shared.Response;
using MediatR;
using OperationResult;

namespace CompraProgramada.Shared.Request;

public record CriarCestaRecomendadaRequest(string Nome, List<ComposicaoCestaDto> Itens)
    : IRequest<Result<CriarCestaRecomendadaResponse>>;