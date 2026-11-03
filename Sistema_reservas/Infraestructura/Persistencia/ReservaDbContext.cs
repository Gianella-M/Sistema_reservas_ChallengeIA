using Sistema_reservas.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Sistema_reservas.Infraestructura.Persistencia;

public class ReservaDbContext : DbContext
{
    public ReservaDbContext(DbContextOptions<ReservaDbContext> options) : base(options) { }

    public DbSet<Unidad> Unidades => Set<Unidad>();
    public DbSet<Reserva> Reservas => Set<Reserva>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Unidad>().HasKey(u => u.Id);

        modelBuilder.Entity<Reserva>(e =>
        {
            e.HasKey(r => r.IdReserva);
            e.Property(r => r.Estado).HasConversion<string>();
            e.HasIndex(r => r.UnidadId);
        });
    }
}
