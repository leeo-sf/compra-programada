using CompraProgramada.Shared.Response;
using MediatR;
using OperationResult;

namespace CompraProgramada.Shared.Request;

public record AdesaoRequest(string Nome, string Cpf, string Email, decimal ValorMensal) : IRequest<Result<AdesaoResponse>>;