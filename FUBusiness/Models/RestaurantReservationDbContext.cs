using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RestaurantBusiness.Models;

namespace FUBusiness.Models;

public partial class RestaurantReservationDbContext : DbContext
{
    public RestaurantReservationDbContext() { }

    public RestaurantReservationDbContext(DbContextOptions<RestaurantReservationDbContext> options)
        : base(options) { }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<Table> Tables { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        IConfigurationRoot configuration = builder.Build();
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Reservat__3214EC27ED947C66");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity
                .Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity
                .Property(e => e.ReservationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.TableId).HasColumnName("TableID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity
                .HasOne(d => d.Table)
                .WithMany(p => p.Reservations)
                .HasForeignKey(d => d.TableId)
                .HasConstraintName("FK__Reservati__Table__4316F928");

            entity
                .HasOne(d => d.User)
                .WithMany(p => p.Reservations)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Reservati__UserI__4222D4EF");
        });

        modelBuilder.Entity<Table>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tables__3214EC27F6F287F2");

            entity.HasIndex(e => e.TableNumber, "UQ__Tables__E8E0DB522B49A109").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity
                .Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC272A13B291");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534F013162B").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity
                .Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Role).HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
