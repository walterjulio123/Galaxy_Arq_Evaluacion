using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Walter.Evaluacion.ApiConsultas.Models
{
    public class Consulta
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? IdConsulta { get; set; }
        public int IdPedido { get; set; }
        public string NombreCliente { get; set; } = string.Empty;
        public int IdPago { get; set; }
        public int FormaPago { get; set; }
        public string NombreFormaPago { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.Decimal128)]
        public decimal MontoPago { get; set; }
    }
}
