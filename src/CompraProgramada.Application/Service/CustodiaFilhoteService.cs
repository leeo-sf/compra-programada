using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Interface;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;
using OperationResult;

namespace CompraProgramada.Application.Service;

public class CustodiaFilhoteService : ICustodiaFilhoteService
{
    private readonly ICustodiaFilhoteRepository _custodiaFilhoteRepository;

    public CustodiaFilhoteService(ICustodiaFilhoteRepository custodiaFilhoteRepository)
        => _custodiaFilhoteRepository = custodiaFilhoteRepository;

    public async Task<Result<List<CustodiaFilhoteDto>>> AtualizarCustodiaFilhoteContasAsync(List<ContaGraficaDto> contas, CancellationToken cancellationToken)
    {
        if (!contas.Any())
            return new ApplicationException("Nenhuma conta gráfica informada para atualização.");

        var contasCustodias = contas.Select(c => new ContaGrafica
        (
            c.Id,
            c.NumeroConta,
            c.DataCriacao,
            c.ClienteId
        )
        {
            CustodiaFilhotes = c.CustodiaFilhote!
                .Select(cf => new CustodiaFilhote(cf.Id, cf.ContaGraficaId, cf.Ticker, cf.PrecoMedio, cf.Quantidade)).ToList()
        }).ToList();

        var custodias = contasCustodias.SelectMany(c => c.CustodiaFilhotes).ToList();

        var custodiasSalvas = await _custodiaFilhoteRepository.AtualizarCustodiasAsync(custodias, cancellationToken);

        return custodiasSalvas.Select(c => new CustodiaFilhoteDto
        {
            Id = c.Id,
            ContaGraficaId = c.ContaGraficaId,
            Ticker = c.Ticker!,
            PrecoMedio = c.PrecoMedio,
            Quantidade = c.Quantidade,
        }).ToList();
    }
}