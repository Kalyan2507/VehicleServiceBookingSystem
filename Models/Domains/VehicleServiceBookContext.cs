using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace VehicleServiceBook.Models.Domains;

public partial class VehicleServiceBookContext : DbContext
{
    public VehicleServiceBookContext()
    {
    }

    public VehicleServiceBookContext(DbContextOptions<VehicleServiceBookContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<Mechanic> Mechanics { get; set; }

    public virtual DbSet<Registration> Registrations { get; set; }

    public virtual DbSet<ServiceCenter> ServiceCenters { get; set; }

    public virtual DbSet<ServiceType> ServiceTypes { get; set; }

    public virtual DbSet<Vehicle> Vehicles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=LTIN617242\\SQLEXPRESS;Database=VehicleServiceBook;Integrated Security=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Bookingid).HasName("PK__Booking__73961EC5F67A7B8A");

            entity.ToTable("Booking");

            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValue("Pending");
            entity.Property(e => e.TimeSlot).HasMaxLength(50);

            entity.HasOne(d => d.ServiceCenter).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.ServiceCenterId)
                .HasConstraintName("FK__Booking__Service__5AEE82B9");

            entity.HasOne(d => d.Registration).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Booking__UserId__59063A47");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.VehicleId)
                .HasConstraintName("FK__Booking__Vehicle__59FA5E80");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("PK__Invoice__D796AAB53F7BFBC9");

            entity.ToTable("Invoice");

            entity.HasIndex(e => e.BookingId, "UQ__Invoice__73951AEC734F6989").IsUnique();

            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(40)
                .IsUnicode(false);

            entity.HasOne(d => d.Booking).WithOne(p => p.Invoice)
                .HasForeignKey<Invoice>(d => d.BookingId)
                .HasConstraintName("FK__Invoice__Booking__6FE99F9F");

            entity.HasOne(d => d.ServiceType).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.ServiceTypeId)
                .HasConstraintName("FK__Invoice__Service__70DDC3D8");
        });

        modelBuilder.Entity<Mechanic>(entity =>
        {
            entity.HasKey(e => e.Mechanicid).HasName("PK__Mechanic__6B0509C952C35A70");

            entity.ToTable("Mechanic");

            entity.Property(e => e.Expertise).HasMaxLength(100);
            entity.Property(e => e.MechanicName).HasMaxLength(100);

            entity.HasOne(d => d.ServiceCenter).WithMany(p => p.Mechanics)
                .HasForeignKey(d => d.ServiceCenterId)
                .HasConstraintName("FK__Mechanic__Servic__5165187F");
        });

        modelBuilder.Entity<Registration>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Registra__1788CC4CFFFF47B6");

            entity.ToTable("Registration");

            entity.HasIndex(e => e.Phone, "UQ__Registra__5C7E359EB11CB881").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Registra__A9D105349468CAF8").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(10);
            entity.Property(e => e.Role).HasMaxLength(20);
        });

        modelBuilder.Entity<ServiceCenter>(entity =>
        {
            entity.HasKey(e => e.ServiceCenterId).HasName("PK__ServiceC__71B62BE37DB8DD79");

            entity.ToTable("ServiceCenter");

            entity.HasIndex(e => e.ServiceCenterContact, "UQ__ServiceC__F2124AAD32FA906C").IsUnique();

            entity.Property(e => e.ServiceCenterContact).HasMaxLength(10);
            entity.Property(e => e.ServiceCenterLocation).HasMaxLength(200);
            entity.Property(e => e.ServiceCenterName).HasMaxLength(100);

            entity.HasOne(d => d.User).WithMany(p => p.ServiceCenters)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__ServiceCe__UserI__4E88ABD4");
        });

        modelBuilder.Entity<ServiceType>(entity =>
        {
            entity.HasKey(e => e.ServiceTypeId).HasName("PK__ServiceT__8ADFAA6C42E00C64");

            entity.ToTable("ServiceType");

            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.VehicleId).HasName("PK__Vehicle__476B5492B47F68A7");

            entity.ToTable("Vehicle");

            entity.Property(e => e.Make).HasMaxLength(100);
            entity.Property(e => e.Model).HasMaxLength(100);
            entity.Property(e => e.RegistrationNumber).HasMaxLength(50);

            entity.HasOne(d => d.User).WithMany(p => p.Vehicles)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Vehicle__UserId__5629CD9C");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
