namespace Walter.Evaluacion.ApiPedidos.DTOs
{
    public class PagoDto
    {
        public int IdPago { get; set; }
        public int IdCliente { get; set; }
        public int FormaPago { get; set; }
        public decimal MontoPago { get; set; }
        public int IdPedido { get; set; }
    }

    public class CreatePagoDto
    {
        public int IdCliente { get; set; }
        public int FormaPago { get; set; }
        public decimal MontoPago { get; set; }
        public int IdPedido { get; set; }
    }
}
