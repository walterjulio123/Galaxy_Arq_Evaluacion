using Walter.Evaluacion.ApiConsultas.DTOs;

namespace Walter.Evaluacion.ApiConsultas.Services
{
    public interface IConsultasService
    {
        Task<IEnumerable<TopicMessageDto>> GetRegistrosAsync();
        Task<ConsultaDto> CreateConsultaAsync(CreateConsultaDto createConsultaDto);
    }
}
