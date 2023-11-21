using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ArchIS6;

public partial class PrintsevAis6Context : DbContext
{
    public PrintsevAis6Context()
    {
    }

    public PrintsevAis6Context(DbContextOptions<PrintsevAis6Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Smartphone> Smartphones { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
        .AddUserSecrets<PrintsevAis6Context>()
        .Build();
        optionsBuilder.UseSqlServer(configuration.GetConnectionString(nameof(PrintsevAis6Context)));
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Cyrillic_General_CI_AS");

        modelBuilder.Entity<Smartphone>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.Cores).HasColumnType("text");
            entity.Property(e => e.Name)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Price).HasColumnType("text");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
