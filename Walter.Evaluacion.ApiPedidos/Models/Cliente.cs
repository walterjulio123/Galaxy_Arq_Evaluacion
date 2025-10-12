using System.ComponentModel.DataAnnotations;

namespace Walter.Evaluacion.ApiPedidos.Models
{
    public class Cliente
    {
        [Key]
        public int IdCliente { get; set; }

        [Required]
        [MaxLength(150)]
        public string NombreCliente { get; set; } = string.Empty;
    }
}
