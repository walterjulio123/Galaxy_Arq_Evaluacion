namespace Walter.Evaluacion.ApiPedidos.DTOs
{
    public class PedidoDto
    {
        public int IdPedido { get; set; }
        public int IdCliente { get; set; }
        public decimal MontoPedido { get; set; }
        public int FormaPago { get; set; }

    }
    public class CreatePedidoDto
    {
        public int IdCliente { get; set; }
        public decimal MontoPedido { get; set; }
        public int FormaPago { get; set; }

    }
}
