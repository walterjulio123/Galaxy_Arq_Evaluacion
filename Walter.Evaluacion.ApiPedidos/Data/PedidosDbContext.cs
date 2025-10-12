using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Walter.Evaluacion.ApiPedidos.Models;

namespace Walter.Evaluacion.ApiPedidos.Data
{
    public class PedidosDbContext : DbContext
    {
        public PedidosDbContext(DbContextOptions<PedidosDbContext> options) : base(options)
        {
        }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Cliente entity
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasKey(e => e.IdCliente);
                entity.Property(e => e.NombreCliente)
                    .IsRequired()
                    .HasMaxLength(150);
            });

            // Configure Pedido entity
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
                entity.Property(e => e.FormaPago)
                    .IsRequired();
            });

            // Seed data
            modelBuilder.Entity<Pedido>().HasData(
                new Pedido { IdPedido = 1, FechaPedido = DateTime.Now.AddDays(-1), IdCliente = 1, MontoPedido = 547.2308M, FormaPago = 1 },
                new Pedido { IdPedido = 2, FechaPedido = DateTime.Now.AddDays(-2), IdCliente = 3, MontoPedido = 12.886M, FormaPago = 3 }
                );

            modelBuilder.Entity<Cliente>().HasData(
                new Cliente { IdCliente = 1, NombreCliente = "Juan Pérez" },
                new Cliente { IdCliente = 2, NombreCliente = "María Luisa García" },
                new Cliente { IdCliente = 3, NombreCliente = "Carlos Rodríguez" },
                new Cliente { IdCliente = 4, NombreCliente = "Ana Martínez" },
                new Cliente { IdCliente = 5, NombreCliente = "Luis Fernández" }
            );
        }
    }
}