using CompraProgramada.Application.Dto;
using CompraProgramada.Domain.Entity;
using OperationResult;

namespace CompraProgramada.Application.Interface;

public interface IContaGraficaService
{
    Task<Result<ContaGraficaDto>> GerarContaGraficaAsync(int clienteId, CancellationToken cancellationToken);
    Task<Result<List<ContaGrafica>>> AtualizarContasAsync(List<ContaGrafica> contas, CancellationToken cancellationToken);
    Task<Result<CarteiraDto>> ObterRentabilidadeDaCertira(List<CustodiaFilhoteDto> custodias, CancellationToken cancellationToken);
    Task<Result<RentabilidadeDto>> ObterEvolucaoDaCertira(ContaGraficaDto conta, CancellationToken cancellationToken);
}