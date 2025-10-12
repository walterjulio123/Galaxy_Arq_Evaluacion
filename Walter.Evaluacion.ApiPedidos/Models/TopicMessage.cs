namespace Walter.Evaluacion.ApiPedidos.Models
{
    public class TopicMessage
    {
        public int IdPedido { get; set; }
        public string NombreCliente { get; set; } = String.Empty;
        public int IdPago { get; set; }
        public int FormaPago { get; set; }
        public decimal MontoPago { get; set; }
    }
}
