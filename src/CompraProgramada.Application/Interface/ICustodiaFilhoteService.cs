using CompraProgramada.Application.Dto;
using OperationResult;

namespace CompraProgramada.Application.Interface;

public interface ICustodiaFilhoteService
{
    Task<Result<CarteiraDto>> ObterRentabilidadeDaCertira(List<CustodiaFilhoteDto> custodias, CancellationToken cancellationToken);
    Task<Result<RentabilidadeDto>> ObterEvolucaoDaCertira(ContaGraficaDto conta, CancellationToken cancellationToken);
}