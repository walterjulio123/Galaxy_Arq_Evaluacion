using Walter.Evaluacion.ApiPedidos.Models;

namespace Walter.Evaluacion.ApiPedidos.Services
{
    public interface ITopicProducerService
    {
        Task<bool> SendMessageAsync(TopicMessage message, CancellationToken cancellationToken = default);
    }
}
