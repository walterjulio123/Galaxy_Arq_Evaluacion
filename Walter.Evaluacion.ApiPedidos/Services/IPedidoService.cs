using Walter.Evaluacion.ApiPedidos.DTOs;

namespace Walter.Evaluacion.ApiPedidos.Services
{
    public interface IPedidoService
    {
        Task<PedidoDto> CreatePedidoAsync(CreatePedidoDto createArticuloDto);
    }
}
