using System.ComponentModel.DataAnnotations;

namespace Walter.Evaluacion.ApiPedidos.Models
{
    public class Pedido
    {
        [Key]
        public int IdPedido { get; set; }

        [Required]
        public DateTime FechaPedido { get; set; }

        [Required]
        public int IdCliente { get; set; }

        [Required]
        public decimal MontoPedido { get; set; }

    }
}
