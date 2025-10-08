using Microsoft.EntityFrameworkCore;
using Walter.Evaluacion.ApiPedidos.Models;

namespace Walter.Evaluacion.ApiPedidos.Data
{
    public class PedidosDbContext : DbContext
    {
        public PedidosDbContext(DbContextOptions<PedidosDbContext> options) : base(options)
        {   
        }
        public DbSet<Pedido> Comprobantes { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Comprobante entity
            modelBuilder.Entity<Pedido>(entity =>
            {
                entity.HasKey(e => e.IdPedido);
                entity.Property(e => e.FechaPedido)
                    .IsRequired();
                entity.Property(e => e.IdCliente)
                    .IsRequired();
                entity.Property(e => e.MontoPedido)
                    .IsRequired()
                    .HasColumnType("decimal(9,2)");
            });

            //// Configure ComprobanteItem entity
            //modelBuilder.Entity<ComprobanteItem>(entity =>
            //{
            //    entity.HasKey(e => e.IdComprobanteItem);
            //    entity.Property(e => e.IdComprobante)
            //        .IsRequired();
            //    entity.Property(e => e.IdArticulo)
            //        .IsRequired();
            //    entity.Property(e => e.Cantidad)
            //        .IsRequired();

            //    // Configure foreign key relationship
            //    entity.HasOne(e => e.Comprobante)
            //        .WithMany(c => c.ComprobanteItems)
            //        .HasForeignKey(e => e.IdComprobante)
            //        .OnDelete(DeleteBehavior.Cascade);
            //});

            //// Seed data
            //modelBuilder.Entity<Comprobante>().HasData(
            //    new Comprobante { IdComprobante = 1, FechaComprobante = DateTime.Now.AddDays(-10), IdCliente = 1 },
            //    new Comprobante { IdComprobante = 2, FechaComprobante = DateTime.Now.AddDays(-8), IdCliente = 2 },
            //    new Comprobante { IdComprobante = 3, FechaComprobante = DateTime.Now.AddDays(-5), IdCliente = 3 },
            //    new Comprobante { IdComprobante = 4, FechaComprobante = DateTime.Now.AddDays(-3), IdCliente = 4 },
            //    new Comprobante { IdComprobante = 5, FechaComprobante = DateTime.Now.AddDays(-1), IdCliente = 5 }
            //);

            //modelBuilder.Entity<ComprobanteItem>().HasData(
            //    new ComprobanteItem { IdComprobanteItem = 1, IdComprobante = 1, IdArticulo = 1, Cantidad = 2 },
            //    new ComprobanteItem { IdComprobanteItem = 2, IdComprobante = 1, IdArticulo = 2, Cantidad = 1 },
            //    new ComprobanteItem { IdComprobanteItem = 3, IdComprobante = 2, IdArticulo = 3, Cantidad = 1 },
            //    new ComprobanteItem { IdComprobanteItem = 4, IdComprobante = 3, IdArticulo = 4, Cantidad = 1 },
            //    new ComprobanteItem { IdComprobanteItem = 5, IdComprobante = 4, IdArticulo = 5, Cantidad = 2 }
            //);
        }

    }
}
