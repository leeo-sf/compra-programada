using CompraProgramada.Application.Config;
using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Interface;
using CompraProgramada.Domain.Entity;
using OperationResult;

namespace CompraProgramada.Application.Service;

public class ImpostoRendaService : IImpostoRendaService
{
    private readonly IKafkaProducer _kafkaProducer;
    private readonly KafkaConfig _kafkaConfig;
    private const decimal ALIQUOTA = 0.00005m;

    public ImpostoRendaService(
        IKafkaProducer kafkaProducer,
        KafkaConfig kafkaConfig)
    {
        _kafkaProducer = kafkaProducer;
        _kafkaConfig = kafkaConfig;
    }

    public async Task<Result<int>> CalcularIRDedoDuro(List<Distribuicao> distribuicoes, CancellationToken cancellationToken)
    {
        var detalhesIr = distribuicoes.Select(dist =>
        {
            var irBruto = dist.ValorOperacao * ALIQUOTA;
            return new IRDedoDuroDto
            {
                ClienteId = dist.ContaGrafica.Cliente.Id,
                Cpf = dist.ContaGrafica.Cliente.Cpf,
                Ticker = dist.Ticker,
                ValorOperacao = dist.ValorOperacao,
                ValorIR = Math.Truncate(irBruto * 100) / 100,
                Data = dist.OrdemCompra.Data
            };
        }).ToList();

        await _kafkaProducer.PublishProducerAsync(_kafkaConfig.TopicIRDedoDuro, detalhesIr);

        return detalhesIr.Count;
    }
}