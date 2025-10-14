using Walter.Evaluacion.ApiPagos.DTOs;

namespace Walter.Evaluacion.ApiPagos.Services
{
    public interface IPagoService
    {
        Task<PagoDto> CreatePagoAsync(CreatePagoDto createPagoDto);
    }
}
