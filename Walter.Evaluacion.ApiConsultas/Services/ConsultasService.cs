using MongoDB.Driver;
using System;
using Walter.Evaluacion.ApiConsultas.Data;
using Walter.Evaluacion.ApiConsultas.DTOs;
using Walter.Evaluacion.ApiConsultas.Enums;
using Walter.Evaluacion.ApiConsultas.Models;

namespace Walter.Evaluacion.ApiConsultas.Services
{
    public class ConsultasService : IConsultasService
    {
        private readonly ConsultasDbContext _context;
        private readonly ILogger<ConsultasService> _logger;
        public ConsultasService(ConsultasDbContext context, ILogger<ConsultasService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<bool> CreateConsultaAsync(CreateConsultaDto createConsultaDto)
        {
            _logger.LogInformation("Creating new consulta with IdPedido: {IdPedido}", createConsultaDto.IdPedido);

            var monto = decimal.Round(createConsultaDto.MontoPago, 2, MidpointRounding.AwayFromZero);

            // validar precision (decimal(9,2) => max 9999999.99)
            if (Math.Abs(monto) > 9999999.99m)
            {
                throw new ArgumentOutOfRangeException(nameof(createConsultaDto.MontoPago), "Monto fuera del rango permitido para decimal(9,2).");
            }
            var consulta = new Consulta
            {
                IdPedido = createConsultaDto.IdPedido,
                NombreCliente = createConsultaDto.NombreCliente,
                IdPago = createConsultaDto.IdPago,
                FormaPago = createConsultaDto.FormaPago,
                NombreFormaPago = createConsultaDto.NombreFormaPago,
                MontoPago = monto
            };

            await _context.Consultas.InsertOneAsync(consulta);

            return consulta != null;
        }

        public async Task<IEnumerable<ConsultaDto>> GetRegistrosAsync()
        {
            _logger.LogInformation("Getting all comprobantes from MongoDB");
            var consultas = await _context.Consultas.Find(_ => true).ToListAsync();
            return consultas.Select(MapToDto);
        }
        private static ConsultaDto MapToDto(Consulta consulta)
        {
            return new ConsultaDto
            {
                IdPedido = consulta.IdPedido,
                NombreCliente = consulta.NombreCliente,
                IdPago = consulta.IdPago,
                FormaPago = consulta.FormaPago,
                NombreFormaPago = consulta.NombreFormaPago,
                MontoPago = consulta.MontoPago
            };
        }
    }
}
