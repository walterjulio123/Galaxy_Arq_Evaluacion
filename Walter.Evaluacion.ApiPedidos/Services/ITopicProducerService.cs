using Walter.Evaluacion.ApiPedidos.DTOs;

namespace Walter.Evaluacion.ApiPedidos.Services
{
    public interface ITopicProducerService
    {
        Task<bool> SendMessageAsync(TopicMessageDto message, CancellationToken cancellationToken = default);
    }
}
