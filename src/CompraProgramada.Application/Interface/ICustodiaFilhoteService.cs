using CompraProgramada.Application.Dto;
using OperationResult;

namespace CompraProgramada.Application.Interface;

public interface ICustodiaFilhoteService
{
    Task<Result<List<CustodiaFilhoteDto>>> AtualizarCustodiaFilhoteContasAsync(List<ContaGraficaDto> contas, CancellationToken cancellationToken);
    Task<Result<CarteiraDto>> ObterRentabilidadeDaCertira(List<CustodiaFilhoteDto> custodias);
}