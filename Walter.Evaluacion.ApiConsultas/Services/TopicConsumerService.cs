
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Walter.Evaluacion.ApiConsultas.Configuration;
using Walter.Evaluacion.ApiConsultas.DTOs;
using Walter.Evaluacion.ApiConsultas.Enums;

namespace Walter.Evaluacion.ApiConsultas.Services
{

    public class TopicConsumerService : BackgroundService
    {
        private readonly IOptions<TopicConfig> topicConfig;
        private readonly ILogger<TopicConsumerService> logger;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly IConsumer<string, string> _consumer;
        public TopicConsumerService(IOptions<TopicConfig> topicConfig,
            ILogger<TopicConsumerService> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            this.topicConfig = topicConfig;
            this.logger = logger;
            this.serviceScopeFactory = serviceScopeFactory;

            var config = new ConsumerConfig
            {
                BootstrapServers = topicConfig.Value.BootstrapServers,
                GroupId = topicConfig.Value.GroupId,
                ClientId = topicConfig.Value.ClientId,
                EnableAutoCommit = true,
                EnableAutoOffsetStore = true,
            };

            _consumer = new ConsumerBuilder<string, string>(config).Build();
        }
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(5000, stoppingToken);

            try
            {
                _consumer.Subscribe(topicConfig.Value.TopicName);

                while (!stoppingToken.IsCancellationRequested)
                {

                    var consumerResult = _consumer.Consume(stoppingToken);

                    if (consumerResult != null)
                    {

                        string json = consumerResult.Message.Value;
                        logger.LogInformation("Consumi el topico. Dato: " + json);
                        var message = JsonSerializer.Deserialize<TopicMessageDto>(json);
                        if (message != null)
                        {
                            using var scope = serviceScopeFactory.CreateScope();
                            var consultaService = scope.ServiceProvider.GetService<IConsultasService>();
                            if (consultaService != null)
                            {
                                // 3. Crear el objeto para MongoDB
                                var consultaMongo = new CreateConsultaDto
                                {
                                    IdPedido = message.IdPedido,
                                    NombreCliente = message.NombreCliente,
                                    IdPago = message.IdPago,
                                    FormaPago = message.FormaPago,
                                    NombreFormaPago = message.FormaPago.GetNombreFormaPago(),
                                    MontoPago = message.MontoPago
                                };

                                // 4. Enviar a Sol.EC.Consultas (MongoDB)
                                var success = await consultaService.CreateConsultaAsync(consultaMongo);
                                if (success)
                                {
                                    logger.LogInformation("Successfully consulta saved");
                                }
                                else
                                {
                                    logger.LogError("Failed to save consulta to Consultas database");
                                }
                            }
                        }


                    }
                    await Task.Delay(100, stoppingToken);

                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "No se pudo consumir el topico");
            }
            finally
            {

                _consumer.Close();
            }
        }
    }
}
