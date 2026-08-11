using CompraProgramada.Shared.Dto;
using CompraProgramada.Domain.Entity;
using OperationResult;
using CompraProgramada.Domain.Contract.Service;
using CompraProgramada.Shared.Config;

namespace CompraProgramada.Domain.Service;

public class ImpostoRendaService : IImpostoRendaService
{
    private readonly IKafkaProducer _kafkaProducer;
    private readonly KafkaConfig _kafkaConfig;

    public ImpostoRendaService(
        IKafkaProducer kafkaProducer,
        AppConfig appConfig)
    {
        _kafkaProducer = kafkaProducer;
        _kafkaConfig = appConfig.Service.Kafka;
    }

    public async Task<Result<int>> PublicarIR(List<Distribuicao> distribuicoes, CancellationToken cancellationToken)
    {
        var detalhesIr = distribuicoes
            .Where(x => CalcularImpostoDeRenda(x.ValorOperacao) > 0)
            .Select(dist => new IRDedoDuroDto
            {
                ClienteId = dist.ContaGrafica.Cliente.Id,
                Cpf = dist.ContaGrafica.Cliente.Cpf,
                Ticker = dist.Ticker,
                ValorOperacao = dist.ValorOperacao,
                ValorIR = CalcularImpostoDeRenda(dist.ValorOperacao),
                Data = dist.OrdemCompra.Data
            }).ToList();

        await _kafkaProducer.PublishProducerAsync(_kafkaConfig.ImpostoRendaTopic, detalhesIr);

        return detalhesIr.Count;
    }

    internal decimal CalcularImpostoDeRenda(decimal valorOperacao)
    {
        var irBruto = valorOperacao * _kafkaConfig.Aliquota;
        return Math.Truncate(irBruto * 100) / 100;
    }
}