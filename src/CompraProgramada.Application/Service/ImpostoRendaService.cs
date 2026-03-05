using CompraProgramada.Application.Config;
using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Interface;
using OperationResult;

namespace CompraProgramada.Application.Service;

public class ImpostoRendaService : IImpostoRendaService
{
    //private readonly IKafkaProducer _kafkaProducer;
    //private readonly KafkaConfig _kafkaConfig;
    private const decimal ALIQUOTA = 0.00005m;

    /*public ImpostoRendaService(
        IKafkaProducer kafkaProducer,
        KafkaConfig kafkaConfig)
    {
        _kafkaProducer = kafkaProducer;
        _kafkaConfig = kafkaConfig;
    }*/

    public async Task<Result> CalcularIRDedoDuro(List<DistribuicaoDto> distribuicoes, CancellationToken cancellationToken)
    {
        var detalhesIr = distribuicoes.Select(d =>
        {
            var irBruto = d.ValorOperacao * ALIQUOTA;
            return new IRDedoDuroDto
            {
                ClienteId = d.ContaGrafica.Id,
                Cpf = d.Cpf,
                Ticker = d.Ticker,
                ValorOperacao = d.ValorOperacao,
                ValorIR = Math.Truncate(irBruto * 100) / 100,
                Data = d.Data
            };
        }).ToList();

        //await _kafkaProducer.PublishProducerAsync(_kafkaConfig.TopicIRDedoDuro, detalhesIr);

        return Result.Success();
    }
}