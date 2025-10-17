using Walter.Evaluacion.ApiConsultas.DTOs;

namespace Walter.Evaluacion.ApiConsultas.Services
{
    public interface IConsultasService
    {
        Task<IEnumerable<ConsultaDto>> GetRegistrosAsync();
        Task<bool> CreateConsultaAsync(CreateConsultaDto createConsultaDto);
    }
}
