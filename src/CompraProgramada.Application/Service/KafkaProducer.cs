using CompraProgramada.Application.Interface;
using Confluent.Kafka;
using System.Text.Json;

namespace CompraProgramada.Application.Service;

public class KafkaProducer : IKafkaProducer
{
    private readonly IProducer<Null, string> _producer;

    public KafkaProducer(ProducerConfig config)
        => _producer = new ProducerBuilder<Null, string>(config).Build();
    
    public async Task PublishProducerAsync<T>(string topic, T message)
    {
        var jsonString = JsonSerializer.Serialize(message);

        var kafkaMessage = new Message<Null, string> { Value = jsonString };

        await _producer.ProduceAsync(topic, kafkaMessage);
    }

    public void Dispose()
    {
        _producer.Flush(TimeSpan.FromSeconds(10));
        _producer.Dispose();
    }
}