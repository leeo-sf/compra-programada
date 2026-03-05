using CompraProgramada.Application.Dto;
using OperationResult;

namespace CompraProgramada.Application.Interface;

public interface IContaGraficaService
{
    Task<Result> AlterarCustodiasAsync(List<ContaGraficaDto> contas, CancellationToken cancellationToken);
    Task<Result<ContaGraficaDto>> GerarContaGraficaAsync(ContaGraficaDto clienteDto, CancellationToken cancellationToken);
}