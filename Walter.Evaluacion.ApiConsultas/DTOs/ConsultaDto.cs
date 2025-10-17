using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Walter.Evaluacion.ApiConsultas.DTOs
{
    public class ConsultaDto
    {
        public string? IdConsulta { get; set; }

        public int IdPedido { get; set; }
        public string NombreCliente { get; set; } = string.Empty;
        public int IdPago { get; set; }
        public int FormaPago { get; set; }
        public string NombreFormaPago { get; set; } = string.Empty;
        public decimal MontoPago { get; set; }

    }

    public class CreateConsultaDto
    {
        public int IdPedido { get; set; }
        public string NombreCliente { get; set; } = string.Empty;
        public int IdPago { get; set; }
        public int FormaPago { get; set; }
        public string NombreFormaPago { get; set; } = string.Empty;
        public decimal MontoPago { get; set; }

    }
}
