using CompraProgramada.Application.Response;
using MediatR;
using OperationResult;
using System.Text.Json.Serialization;

namespace CompraProgramada.Application.Request;

public record ExecutarCompraRequest(
    [property: JsonIgnore] DateTime DataSolicitacao,
    DateOnly DataReferencia) : IRequest<Result<ExecutarCompraResponse>>;