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

    public async Task<Result<ContaGraficaDto>> GerarContaGraficaAsync(ContaGraficaDto contaGraficaDto, CancellationToken cancellationToken)
    {
        var cestaVigente = await _cestaRecomendadaService.ObterCestaAtivaAsync(cancellationToken);

        var custodiaConta = cestaVigente.Value!.Itens
            .Select(x => new CustodiaFilhote(0, 0, x.Ticker)).ToList();

        var numeroConta = GerarNumeroConta(contaGraficaDto.ClienteId, ehContaGrafica: true);
        var novaConta = new ContaGrafica(0, numeroConta, DateTime.Now, contaGraficaDto.ClienteId) { CustodiaFilhotes = custodiaConta };

        var contaSalva = await _contaGraficaRepository.CriarAsync(novaConta, cancellationToken);

        return new ContaGraficaDto
        {
            Id = contaSalva.Id,
            NumeroConta = contaSalva.NumeroConta,
            DataCriacao = contaSalva.DataCriacao,
            ClienteId = contaSalva.ClienteId,
            Tipo = contaSalva.Tipo,
            CustodiaFilhote = contaSalva.CustodiaFilhotes.Select(cf => new CustodiaFilhoteDto
            {
                Id = cf.Id,
                ContaGraficaId = cf.ContaGraficaId,
                Ticker = cf.Ticker ?? string.Empty,
                Quantidade = cf.Quantidade
            }).ToList()
        };
    }

    public async Task<Result> AlterarCustodiasAsync(List<ContaGraficaDto> contasDto, CancellationToken cancellationToken)
    {
        if (!contasDto.Any())
            return new ApplicationException("Nenhuma conta foi informada.");

        var contas = contasDto
            .Select(c => new ContaGrafica(c.Id, c.NumeroConta, c.DataCriacao, c.ClienteId)
            {
                CustodiaFilhotes = c.CustodiaFilhote?
                    .Select(cf => new CustodiaFilhote(cf.Id, cf.ContaGraficaId, cf.Ticker, cf.Quantidade)).ToList() ?? []
            }).ToList();

        await _contaGraficaRepository.AtualizarCustodiasAysnc(contas, cancellationToken);

        return Result.Success();
    }

    private string GerarNumeroConta(int id, bool ehContaGrafica)
    {
        var prefixo = ehContaGrafica ? "FLH" : "MST";
        return $"{prefixo}-{id:D6}";
    }
}