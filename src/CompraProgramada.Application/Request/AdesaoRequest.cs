using CompraProgramada.Application.Response;
using MediatR;
using OperationResult;

namespace CompraProgramada.Application.Request;

public record AdesaoRequest(string Nome, string Cpf, string Email, decimal ValorMensal) : IRequest<Result<AdesaoResponse>>;