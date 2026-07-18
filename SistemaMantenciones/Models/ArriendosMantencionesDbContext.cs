using Microsoft.EntityFrameworkCore;

namespace SistemaMantenciones.Models;

public partial class ArriendosMantencionesDbContext : DbContext
{
    public ArriendosMantencionesDbContext()
    {
    }

    public ArriendosMantencionesDbContext(DbContextOptions<ArriendosMantencionesDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Mantenicion> manteniciones { get; set; } = null!;
    public virtual DbSet<Vehiculo> vehiculos { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Mantenicion>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");
            entity.ToTable("mantenicion");

            entity.HasIndex(e => e.codigoVehiculo, "codigoVehiculo");

            entity.Property(e => e.id).HasColumnName("id");
            entity.Property(e => e.codigoVehiculo).HasMaxLength(20).HasColumnName("codigoVehiculo");
            entity.Property(e => e.fecha).HasColumnType("datetime").HasColumnName("fecha");
            entity.Property(e => e.horas).HasColumnName("horas");
            entity.Property(e => e.descripcion).HasMaxLength(200).HasColumnName("descripcion");

            entity.HasOne(d => d.codigoVehiculoNavigation).WithMany(p => p.manteniciones)
                .HasForeignKey(d => d.codigoVehiculo)
                .HasConstraintName("mantenicion_ibfk_1");
        });

        modelBuilder.Entity<Vehiculo>(entity =>
        {
            entity.HasKey(e => e.codigo).HasName("PRIMARY");
            entity.ToTable("vehiculo");

            entity.HasIndex(e => e.patente, "patente").IsUnique();

            entity.Property(e => e.codigo).HasMaxLength(20).HasColumnName("codigo");
            entity.Property(e => e.estado).HasMaxLength(30).HasColumnName("estado");
            entity.Property(e => e.marca).HasMaxLength(50).HasColumnName("marca");
            entity.Property(e => e.modelo).HasMaxLength(50).HasColumnName("modelo");
            entity.Property(e => e.patente).HasMaxLength(10).HasColumnName("patente");
            entity.Property(e => e.tipo).HasMaxLength(50).HasColumnName("tipo");
            entity.Property(e => e.kilometraje).HasColumnName("kilometraje");
            entity.Property(e => e.precioArriendoDiario).HasColumnName("precioArriendoDiario");
        });
    }
}
