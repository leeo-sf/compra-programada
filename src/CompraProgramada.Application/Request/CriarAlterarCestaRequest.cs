using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Response;
using MediatR;
using OperationResult;

namespace CompraProgramada.Application.Request;

public record CriarAlterarCestaRequest(string Nome, List<ComposicaoCestaDto> Itens) : IRequest<Result<CriarAlterarCestaResponse>>;