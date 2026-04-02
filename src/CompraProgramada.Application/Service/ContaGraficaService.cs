using CompraProgramada.Application.Contract.Service;
using CompraProgramada.Domain.Entity;
using CompraProgramada.Domain.Interface;
using OperationResult;

namespace CompraProgramada.Application.Service;

public class ContaGraficaService : IContaGraficaService
{
    private readonly IContaGraficaRepository _contaGraficaRepository;

    public ContaGraficaService(IContaGraficaRepository contaGraficaRepository) => _contaGraficaRepository = contaGraficaRepository;

    public async Task<Result<ContaGrafica>> GerarContaGraficaAsync(Cliente cliente, CancellationToken cancellationToken)
    {
        var conta = ContaGrafica.Gerar(cliente);

        var contaSalva = await _contaGraficaRepository.CriarAsync(conta, cancellationToken);

        return contaSalva;
    }

    public async Task<Result<List<ContaGrafica>>> AtualizarContasAsync(List<ContaGrafica> contasAhSeremAtualizadas, CancellationToken cancellationToken)
    {
        if (!contasAhSeremAtualizadas.Any())
            return new ApplicationException("Nenhuma conta gráfica informada para atualização.");

        var contasSalvas = await _contaGraficaRepository.AtualizarContasAsync(contasAhSeremAtualizadas, cancellationToken);

        return contasSalvas;
    }
}