using CompraProgramada.Shared.Dto;
using CompraProgramada.Domain.Entity;
using OperationResult;
using CompraProgramada.Application.Contract.Service;

namespace CompraProgramada.Application.Service;

public class ImpostoRendaService : IImpostoRendaService
{
    private readonly IKafkaProducer _kafkaProducer;
    private const string TOPIC_IR_DEDO_DURO = "topic.irdedo-duro";
    private const decimal ALIQUOTA = 0.00005m;

    public ImpostoRendaService(IKafkaProducer kafkaProducer) => _kafkaProducer = kafkaProducer;

    public async Task<Result<int>> PublicarIR(List<Distribuicao> distribuicoes, CancellationToken cancellationToken)
    {
        var detalhesIr = distribuicoes
            .Where(x => CalcularImpostoDeRenda(x.ValorOperacao) > 0)
            .Select(dist =>
        {
            return new IRDedoDuroDto
            {
                ClienteId = dist.ContaGrafica.Cliente.Id,
                Cpf = dist.ContaGrafica.Cliente.Cpf,
                Ticker = dist.Ticker,
                ValorOperacao = dist.ValorOperacao,
                ValorIR = CalcularImpostoDeRenda(dist.ValorOperacao),
                Data = dist.OrdemCompra.Data
            };
        }).ToList();

        await _kafkaProducer.PublishProducerAsync(TOPIC_IR_DEDO_DURO, detalhesIr);

        return detalhesIr.Count;
    }

    public decimal CalcularImpostoDeRenda(decimal valorOperacao)
    {
        var irBruto = valorOperacao * ALIQUOTA;
        return Math.Truncate(irBruto * 100) / 100;
    }
}