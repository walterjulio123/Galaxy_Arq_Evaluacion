using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Walter.Evaluacion.ApiPedidos.Configuration;
using Walter.Evaluacion.ApiPedidos.Models;

namespace Walter.Evaluacion.ApiPedidos.Services
{
    public class TopicProducerService : ITopicProducerService
    {
        private readonly IOptions<TopicConfig> kafkaConfig;
        private readonly ILogger<TopicProducerService> logger;
        private readonly IProducer<string, string> _producer;
        public TopicProducerService(IOptions<TopicConfig> kafkaConfig, ILogger<TopicProducerService> logger)
        {
            this.kafkaConfig = kafkaConfig;
            this.logger = logger;

            var config = new ProducerConfig
            {
                BootstrapServers = kafkaConfig.Value.BootstrapServers,
                ClientId = kafkaConfig.Value.ClientId,
                Acks = Acks.All
            };

            _producer = new ProducerBuilder<string, string>(config).Build();
        }
        public async Task<bool> SendMessageAsync(TopicMessage message, CancellationToken cancellationToken = default)
        {
            var messageJson = JsonSerializer.Serialize(message);

            var kafkaMessage = new Message<string, string>
            {
                Key = message.IdPago.ToString(),
                Value = messageJson,
                Timestamp = Timestamp.Default
            };


            try
            {
                var result = await _producer.ProduceAsync(kafkaConfig.Value.TopicName, kafkaMessage, cancellationToken);

                logger.LogInformation("Mensaje enviado a Kafka");
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al enviar el mensaje");
                return false;
            }
        }
    }
}
