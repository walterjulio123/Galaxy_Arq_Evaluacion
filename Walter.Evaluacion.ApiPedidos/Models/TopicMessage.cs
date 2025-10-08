namespace Walter.Evaluacion.ApiPedidos.Models
{
    public class TopicMessage
    {
        //public int Codigo { get; set; }
        //public string Nombre { get; set; } = String.Empty;


        public int IdPedido { get; set; }
        public string NombreCliente { get; set; } = String.Empty;
        public int IdPago { get; set; }
        public double Montopago { get; set; }
    }
}
