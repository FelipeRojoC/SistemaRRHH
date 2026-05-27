using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace SistemaRRHH.Models;

public partial class RrhhDbContext : DbContext
{
    public RrhhDbContext()
    {
    }

    public RrhhDbContext(DbContextOptions<RrhhDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Mecanico> Mecanicos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Mecanico>(entity =>
        {
            entity.HasKey(e => e.Rut).HasName("PRIMARY");

            entity.ToTable("Mecanico");

            entity.Property(e => e.Rut).HasMaxLength(12);
            entity.Property(e => e.Nombre).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
