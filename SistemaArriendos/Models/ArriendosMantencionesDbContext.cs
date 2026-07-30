using Microsoft.EntityFrameworkCore;

namespace SistemaArriendos.Models;

public partial class ArriendosMantencionesDbContext : DbContext
{
    public ArriendosMantencionesDbContext()
    {
    }

    public ArriendosMantencionesDbContext(DbContextOptions<ArriendosMantencionesDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Arriendo> arriendos { get; set; } = null!;
    public virtual DbSet<Cliente> clientes { get; set; } = null!;
    public virtual DbSet<VehiculoCache> vehiculosCache { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Arriendo>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");
            entity.ToTable("arriendo");

            entity.HasIndex(e => e.rutCliente, "rutCliente");

            entity.Property(e => e.id).HasColumnName("id");
            entity.Property(e => e.codigoVehiculo).HasMaxLength(20).HasColumnName("codigoVehiculo");
            entity.Property(e => e.fechaFin).HasColumnType("datetime").HasColumnName("fechaFin");
            entity.Property(e => e.fechaInicio).HasColumnType("datetime").HasColumnName("fechaInicio");
            entity.Property(e => e.rutCliente).HasMaxLength(12).HasColumnName("rutCliente");
            entity.Property(e => e.precioDiario).HasColumnName("precioDiario");
            entity.Property(e => e.precioTotal).HasColumnName("precioTotal");
            entity.Property(e => e.estado).HasMaxLength(20).HasColumnName("estado");

            entity.HasOne(d => d.rutClienteNavigation).WithMany(p => p.arriendos)
                .HasForeignKey(d => d.rutCliente)
                .HasConstraintName("arriendo_ibfk_2");
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.rut).HasName("PRIMARY");
            entity.ToTable("cliente");

            entity.Property(e => e.rut).HasMaxLength(12).HasColumnName("rut");
            entity.Property(e => e.direccion).HasMaxLength(200).HasColumnName("direccion");
            entity.Property(e => e.nombre).HasMaxLength(100).HasColumnName("nombre");
        });

        modelBuilder.Entity<VehiculoCache>(entity =>
        {
            entity.HasKey(e => e.codigo).HasName("PRIMARY");
            entity.ToTable("vehiculo_cache");

            entity.Property(e => e.codigo).HasMaxLength(20).HasColumnName("codigo");
            entity.Property(e => e.patente).HasMaxLength(10).HasColumnName("patente");
            entity.Property(e => e.marca).HasMaxLength(50).HasColumnName("marca");
            entity.Property(e => e.modelo).HasMaxLength(50).HasColumnName("modelo");
            entity.Property(e => e.tipo).HasMaxLength(50).HasColumnName("tipo");
            entity.Property(e => e.kilometraje).HasColumnName("kilometraje");
            entity.Property(e => e.estado).HasMaxLength(30).HasColumnName("estado");
            entity.Property(e => e.precioArriendoDiario).HasColumnName("precioArriendoDiario");
        });
    }
}
