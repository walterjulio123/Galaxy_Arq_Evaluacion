using Microsoft.Extensions.Logging;
using Walter.Evaluacion.ApiPedidos.Data;
using Walter.Evaluacion.ApiPedidos.DTOs;
using Walter.Evaluacion.ApiPedidos.Models;

namespace Walter.Evaluacion.ApiPedidos.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly PedidosDbContext _context;
        private readonly ILogger<PedidoService> _logger;

        public PedidoService(PedidosDbContext context, ILogger<PedidoService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PedidoDto> CreatePedidoAsync(CreatePedidoDto createPedidoDto)
        {
            _logger.LogInformation("Creating new pedido: {IdCliente}", createPedidoDto.IdCliente);
            var pedido = new Pedido
            {
                IdCliente = createPedidoDto.IdCliente,
                FormaPago = createPedidoDto.FormaPago,
                MontoPedido = createPedidoDto.MontoPedido,
                FechaPedido= DateTime.UtcNow
            };

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            return new PedidoDto
            {
                IdPedido = pedido.IdPedido,
                IdCliente = pedido.IdCliente,
                FormaPago = pedido.FormaPago,
                MontoPedido = pedido.MontoPedido
            };
        }
    }
}
