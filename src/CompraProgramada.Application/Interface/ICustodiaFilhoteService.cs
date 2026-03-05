using CompraProgramada.Application.Dto;
using OperationResult;

namespace CompraProgramada.Application.Interface;

public interface ICustodiaFilhoteService
{
    Task<Result<List<CustodiaFilhoteDto>>> AtualizarCustodiaFilhoteContasAsync(List<ContaGraficaDto> contas, CancellationToken cancellationToken);
}