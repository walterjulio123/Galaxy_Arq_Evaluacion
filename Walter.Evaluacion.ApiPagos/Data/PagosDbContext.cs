using Microsoft.EntityFrameworkCore;
using Walter.Evaluacion.ApiPagos.Models;

namespace Walter.Evaluacion.ApiPagos.Data
{
    public class PagosDbContext : DbContext
    {
        public PagosDbContext(DbContextOptions<PagosDbContext> options) : base(options)
        {
        }

        public DbSet<Pago> Pagos { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Comprobante entity
            modelBuilder.Entity<Pago>(entity =>
            {
                entity.HasKey(e => e.IdPago);
                entity.Property(e => e.FechaPago)
                    .IsRequired();
                entity.Property(e => e.IdCliente)
                    .IsRequired();
                entity.Property(e => e.FormaPago)
                    .IsRequired();
            });
        }
    }
}
