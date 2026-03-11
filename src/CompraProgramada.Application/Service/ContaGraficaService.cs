using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Interface;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;
using OperationResult;

namespace CompraProgramada.Application.Service;

public class ContaGraficaService : IContaGraficaService
{
    private readonly ICestaRecomendadaService _cestaRecomendadaService;
    private readonly IContaGraficaRepository _contaGraficaRepository;

    public ContaGraficaService(ICestaRecomendadaService cestaRecomendadaService,
        IContaGraficaRepository contaGraficaRepository)
    {
        _cestaRecomendadaService = cestaRecomendadaService;
        _contaGraficaRepository = contaGraficaRepository;
    }

    public async Task<Result<ContaGraficaDto>> GerarContaGraficaAsync(int clienteId, CancellationToken cancellationToken)
    {
        var cestaVigente = await _cestaRecomendadaService.ObterCestaAtivaAsync(cancellationToken);
        if (!cestaVigente.IsSuccess)
            return cestaVigente.Exception;

        var custodiasConta = cestaVigente.Value!.Itens
            .Select(x => CustodiaFilhote.GerarCustodia(x.Ticker)).ToList();

        var conta = ContaGrafica.Gerar(clienteId, custodiasConta);

        var contaSalva = await _contaGraficaRepository.CriarAsync(conta, cancellationToken);

        return new ContaGraficaDto(
            contaSalva.Id,
            contaSalva.NumeroConta,
            contaSalva.DataCriacao,
            contaSalva.ClienteId,
            contaSalva.Tipo,
            null,
            contaSalva.CustodiaFilhotes.Select(cf => new CustodiaFilhoteDto(
                cf.Id,
                cf.ContaGraficaId,
                cf.Ticker,
                cf.PrecoMedio,
                cf.Quantidade
            )).ToList()
        );
    }

    public async Task<Result> RegistrarComprasAsync(List<HistoricoCompraDto> compras, CancellationToken cancellationToken)
    {
        if (!compras.Any())
            return new ApplicationException("Nenhuma compra informada para registro.");

        var comprarAhRegistrar = compras.Select(hc => HistoricoCompra.RegistrarHistorico(hc.ContaGraficaId, hc.Ticker, hc.Quantidade, hc.PrecoExecutado, hc.PrecoMedio, hc.ValorAporte, hc.Data)).ToList();

        await _contaGraficaRepository.RegistrarHistoricoCompraAysnc(comprarAhRegistrar, cancellationToken);

        return Result.Success();
    }

    public async Task<Result<List<CustodiaFilhoteDto>>> AtualizarCustodiasContasAsync(List<ContaGraficaDto> contasAhSeremAtualizadas, CancellationToken cancellationToken)
    {
        if (!contasAhSeremAtualizadas.Any())
            return new ApplicationException("Nenhuma conta gráfica informada para atualização.");

        var contas = await _contaGraficaRepository.ObterContasAtivas(cancellationToken);

        var contasAtualizadas = contas.Select(conta =>
        {
            var contaDto = contasAhSeremAtualizadas.FirstOrDefault(c => c.NumeroConta == conta.NumeroConta);
            var custodiasDto = contaDto?.CustodiaFilhotes;

            custodiasDto?.Select(ct =>
            {
                var custodia = conta.CustodiaFilhotes.FirstOrDefault(x => x.ContaGraficaId == ct.ContaGraficaId);
                custodia?.Atualizar(ct.PrecoMedio, ct.Quantidade);
                return custodia;
            }).ToList();

            return conta;

        }).ToList();

        var custodias = contasAtualizadas.SelectMany(c => c.CustodiaFilhotes).ToList();

        var custodiasSalvas = await _contaGraficaRepository.AtualizarCustodiasAsync(custodias, cancellationToken);

        return custodiasSalvas.Select(c => new CustodiaFilhoteDto(
            c.Id,
            c.ContaGraficaId,
            c.Ticker!,
            c.PrecoMedio,
            c.Quantidade
        )).ToList();
    }
}