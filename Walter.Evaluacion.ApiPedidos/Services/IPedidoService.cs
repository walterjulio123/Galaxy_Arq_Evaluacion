using Walter.Evaluacion.ApiPedidos.DTOs;

namespace Walter.Evaluacion.ApiPedidos.Services
{
    public interface IPedidoService
    {
        Task<PedidoDto> CreatePedidoAsync(CreatePedidoDto createArticuloDto);
        Task<ClienteDto> GetClienteByIdAsync(int id);
        Task<IEnumerable<ClienteDto>> GetClientesAsync();
        Task<int> CreatePagoAsync(CreatePagoDto createPagoDto);
    }
}
