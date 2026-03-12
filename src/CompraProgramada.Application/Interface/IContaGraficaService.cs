using CompraProgramada.Application.Dto;
using OperationResult;

namespace CompraProgramada.Application.Interface;

public interface IContaGraficaService
{
    Task<Result<ContaGraficaDto>> GerarContaGraficaAsync(int clienteId, CancellationToken cancellationToken);
    Task<Result> RegistrarComprasAsync(List<HistoricoCompraDto> compras, CancellationToken cancellationToken);
    Task<Result<List<CustodiaFilhoteDto>>> AtualizarCustodiasContasAsync(List<ContaGraficaDto> contas, CancellationToken cancellationToken);
    Task<Result<CarteiraDto>> ObterRentabilidadeDaCertira(List<CustodiaFilhoteDto> custodias, CancellationToken cancellationToken);
    Task<Result<RentabilidadeDto>> ObterEvolucaoDaCertira(ContaGraficaDto conta, CancellationToken cancellationToken);
}