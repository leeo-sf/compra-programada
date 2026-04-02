using CompraProgramada.Shared.Response;
using MediatR;
using OperationResult;
using System.Text.Json.Serialization;

namespace CompraProgramada.Shared.Request;

public record ExecutarCompraRequest(
    [property: JsonIgnore] DateTime DataSolicitacao,
    DateOnly DataReferencia) : IRequest<Result<ExecutarCompraResponse>>;