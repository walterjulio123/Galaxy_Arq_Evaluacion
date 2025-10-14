using Walter.Evaluacion.ApiPagos.Data;
using Walter.Evaluacion.ApiPagos.DTOs;
using Walter.Evaluacion.ApiPagos.Models;

namespace Walter.Evaluacion.ApiPagos.Services
{
    public class PagoService : IPagoService
    {
        private readonly PagosDbContext _context;
        private readonly ILogger<PagoService> _logger;

        public PagoService(PagosDbContext context, ILogger<PagoService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<PagoDto> CreatePagoAsync(CreatePagoDto createPagoDto)
        {
            _logger.LogInformation("Creating new pago: {IdPedido}", createPagoDto.IdPedido);
            var pago = new Pago
            {
                IdCliente = createPagoDto.IdCliente,
                FormaPago = createPagoDto.FormaPago,
                MontoPago = createPagoDto.MontoPago,
                IdPedido = createPagoDto.IdPedido,
                FechaPago= DateTime.UtcNow
            };

            _context.Pagos.Add(pago);
            await _context.SaveChangesAsync();

            return new PagoDto
            {
                IdPago = pago.IdPago,
                IdCliente = pago.IdCliente,
                FormaPago = pago.FormaPago,
                MontoPago = pago.MontoPago,
                IdPedido = pago.IdPedido
            };
        }
    }
}
