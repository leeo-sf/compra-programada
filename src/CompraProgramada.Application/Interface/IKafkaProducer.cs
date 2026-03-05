namespace CompraProgramada.Application.Interface;

public interface IKafkaProducer
{
    Task PublishProducerAsync<T>(string topic, T message);
}