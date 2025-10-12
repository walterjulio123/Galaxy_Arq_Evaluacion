using System.ComponentModel.DataAnnotations;

namespace Walter.Evaluacion.ApiPagos.Models
{
    public class Pago
    {
        [Key]
        public int IdPago { get; set; }

        [Required]
        public DateTime FechaPago { get; set; }

        [Required]
        public int IdCliente { get; set; }

        [Required]
        public int FormaPago { get; set; }

        [Required]
        public int IdPedido { get; set; }

        [Required]
        public decimal MontoPago { get; set; }
    }
}
