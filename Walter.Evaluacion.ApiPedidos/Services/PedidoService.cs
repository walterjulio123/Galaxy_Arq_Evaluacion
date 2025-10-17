using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
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
        private readonly IConfiguration _configuration;
        private readonly IHttpClientService _httpClientService;

        public PedidoService(IHttpClientService httpClientService,
            PedidosDbContext context,
            ILogger<PedidoService> logger,
            IConfiguration configuration)
        {
            _httpClientService = httpClientService;
            _context = context;
            _logger = logger;
            _configuration = configuration;
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

        public async Task<ClienteDto> GetClienteByIdAsync(int id)
        {
            _logger.LogInformation("Getting a cliente by id");
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
                return null;

            return new ClienteDto
            {
                IdCliente = cliente.IdCliente,
                NombreCliente = cliente.NombreCliente
            };
        }

        public async Task<IEnumerable<ClienteDto>> GetClientesAsync()
        {
            _logger.LogInformation("Getting all clients");
            var clientes = await _context.Clientes.ToListAsync();
            return clientes.Select(c => new ClienteDto
            {
                IdCliente = c.IdCliente,
                NombreCliente = c.NombreCliente
            });
        }

        public async Task<PagoDto> CreatePagoAsync(CreatePagoDto createPagoDto)
        {
            var pagoBaseUrl = _configuration["Services:Pago:BaseUrl"] ?? "http://localhost:5102";
            var url = $"{pagoBaseUrl}/pago";
            return await _httpClientService.PostAsync<PagoDto>(url, createPagoDto);
        }
    }
}
