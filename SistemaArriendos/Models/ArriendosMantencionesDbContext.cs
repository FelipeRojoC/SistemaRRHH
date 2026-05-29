using System;
using System.Collections.Generic;
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

    public virtual DbSet<Arriendo> Arriendos { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<Mantencion> Mantencions { get; set; }

    public virtual DbSet<Vehiculo> Vehiculos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Arriendo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("arriendo");

            entity.HasIndex(e => e.CodigoVehiculo, "CodigoVehiculo");

            entity.HasIndex(e => e.RutCliente, "RutCliente");

            entity.Property(e => e.CodigoVehiculo).HasMaxLength(20);
            entity.Property(e => e.FechaFin).HasColumnType("datetime");
            entity.Property(e => e.FechaInicio).HasColumnType("datetime");
            entity.Property(e => e.RutCliente).HasMaxLength(12);

            entity.HasOne(d => d.CodigoVehiculoNavigation).WithMany(p => p.Arriendos)
                .HasForeignKey(d => d.CodigoVehiculo)
                .HasConstraintName("arriendo_ibfk_1");

            entity.HasOne(d => d.RutClienteNavigation).WithMany(p => p.Arriendos)
                .HasForeignKey(d => d.RutCliente)
                .HasConstraintName("arriendo_ibfk_2");
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.Rut).HasName("PRIMARY");

            entity.ToTable("cliente");

            entity.Property(e => e.Rut).HasMaxLength(12);
            entity.Property(e => e.Direccion).HasMaxLength(200);
            entity.Property(e => e.Nombre).HasMaxLength(100);
        });

        modelBuilder.Entity<Mantencion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("mantencion");

            entity.HasIndex(e => e.CodigoVehiculo, "CodigoVehiculo");

            entity.Property(e => e.CodigoVehiculo).HasMaxLength(20);
            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.RutMecanico).HasMaxLength(12);

            entity.HasOne(d => d.CodigoVehiculoNavigation).WithMany(p => p.Mantencions)
                .HasForeignKey(d => d.CodigoVehiculo)
                .HasConstraintName("mantencion_ibfk_1");
        });

        modelBuilder.Entity<Vehiculo>(entity =>
        {
            entity.HasKey(e => e.Codigo).HasName("PRIMARY");

            entity.ToTable("vehiculo");

            entity.HasIndex(e => e.Patente, "Patente").IsUnique();

            entity.Property(e => e.Codigo).HasMaxLength(20);
            entity.Property(e => e.Estado).HasMaxLength(30);
            entity.Property(e => e.Marca).HasMaxLength(50);
            entity.Property(e => e.Modelo).HasMaxLength(50);
            entity.Property(e => e.Patente).HasMaxLength(10);
            entity.Property(e => e.Tipo).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
