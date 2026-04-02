namespace CompraProgramada.Application.Contract.Service;

public interface IKafkaProducer
{
    Task PublishProducerAsync<T>(string topic, T message);
}