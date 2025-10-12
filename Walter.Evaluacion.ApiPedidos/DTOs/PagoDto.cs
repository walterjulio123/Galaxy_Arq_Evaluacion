namespace Walter.Evaluacion.ApiPedidos.DTOs
{
    public class PagoDto
    {
        public int IdCliente { get; set; }
        public int FormaPago { get; set; }
        public decimal MontoPago { get; set; }
    }
}
