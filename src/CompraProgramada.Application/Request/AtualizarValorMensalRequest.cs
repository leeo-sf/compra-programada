using CompraProgramada.Application.Response;
using MediatR;
using OperationResult;
using System.Text.Json.Serialization;

namespace CompraProgramada.Application.Request;

public record AtualizarValorMensalRequest(
    [property: JsonIgnore] int ClienteId,
    decimal NovoValorMensal) : IRequest<Result<AtualizarValorMensalResponse>>;