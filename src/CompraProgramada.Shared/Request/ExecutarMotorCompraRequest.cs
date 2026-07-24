using CompraProgramada.Shared.Response;
using MediatR;
using OperationResult;
using System.Text.Json.Serialization;

namespace CompraProgramada.Shared.Request;

public record ExecutarMotorCompraRequest(DateOnly? DataReferencia) : IRequest<Result<ExecutarMotorCompraResponse>>;