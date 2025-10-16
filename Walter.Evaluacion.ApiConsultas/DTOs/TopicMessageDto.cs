namespace Walter.Evaluacion.ApiConsultas.DTOs
{
    public class TopicMessageDto
    {
        public int IdPedido { get; set; }
        public string NombreCliente { get; set; } = string.Empty;
        public int IdPago { get; set; }
        public int FormaPago { get; set; }
        public decimal MontoPago { get; set; }
    }
}
