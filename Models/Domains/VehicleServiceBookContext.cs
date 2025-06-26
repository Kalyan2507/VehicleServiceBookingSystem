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
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Only configure SQL Server if no provider was set (e.g., in production)
            optionsBuilder.UseSqlServer("Server=LTIN617505\\SQLEXPRESS;Database=VehicleServiceBook;Trusted_Connection=True;TrustServerCertificate=True");
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Registration>(entity =>

        {

            entity.HasKey(e => e.UserId).HasName("PK__Registra__1788CC4C88AA8157");

            entity.ToTable("Registration");

            entity.HasIndex(e => e.Email, "UQ__Registra__A9D10534FDAE5997").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(200);

            entity.Property(e => e.Email).HasMaxLength(100);

            entity.Property(e => e.Name).HasMaxLength(100);

            entity.Property(e => e.PasswordHash).HasMaxLength(50);

            entity.Property(e => e.Phone).HasMaxLength(20);

            entity.Property(e => e.Role).HasMaxLength(20);

        });

        // ServiceCenter

        modelBuilder.Entity<ServiceCenter>(entity =>

        {

            entity.HasKey(e => e.ServiceCenterId).HasName("PK__ServiceC__71B62BE31C4F7EA0");

            entity.ToTable("ServiceCenter");

            entity.Property(e => e.ServiceCenterContact).HasMaxLength(20);

            entity.Property(e => e.ServiceCenterLocation).HasMaxLength(200);

            entity.Property(e => e.ServiceCenterName).HasMaxLength(100);

            entity.HasOne(d => d.User)

                .WithMany(p => p.ServiceCenters)

                .HasForeignKey(d => d.UserId)

                .HasConstraintName("FK__ServiceCe__UserI__3A81B327");

        });

        // Mechanic

        modelBuilder.Entity<Mechanic>(entity =>

        {

            entity.HasKey(e => e.Mechanicid).HasName("PK__Mechanic__6B0509C9F62A2D41");

            entity.ToTable("Mechanic");

            entity.Property(e => e.Expertise).HasMaxLength(100);

            entity.Property(e => e.MechanicName).HasMaxLength(100);

            entity.HasOne(m => m.ServiceCenter)

                .WithMany(sc => sc.Mechanics)

                .HasForeignKey(m => m.ServiceCenterId)

                .HasConstraintName("FK_Mechanic_ServiceCenter");

        });

        // ServiceType

        modelBuilder.Entity<ServiceType>(entity =>

        {

            entity.HasKey(e => e.ServiceTypeId).HasName("PK__ServiceT__8ADFAA6C2546C593");

            entity.ToTable("ServiceType");

            entity.Property(e => e.Description).HasMaxLength(200);

            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");

        });

        // Vehicle

        modelBuilder.Entity<Vehicle>(entity =>

        {

            entity.HasKey(e => e.VehicleId).HasName("PK__Vehicle__476B5492C88E34FF");

            entity.ToTable("Vehicle");

            entity.Property(e => e.Make).HasMaxLength(100);

            entity.Property(e => e.Model).HasMaxLength(100);

            entity.Property(e => e.RegistrationNumber).HasMaxLength(50);

            entity.HasOne(v => v.User)

                .WithMany(u => u.Vehicles)

                .HasForeignKey(v => v.UserId)

                .HasConstraintName("FK_Vehicle_Registration");

        });

        // Booking

        modelBuilder.Entity<Booking>(entity =>

        {

            entity.HasKey(e => e.Bookingid).HasName("PK__Booking__73961EC5AADDA612");

            entity.ToTable("Booking");

            entity.Property(e => e.Date).HasColumnType("datetime");

            entity.Property(e => e.TimeSlot).HasMaxLength(50);

            entity.Property(e => e.Status)

                  .HasMaxLength(30)

                  .HasDefaultValue("Pending");

            entity.HasOne(b => b.Registration)

                .WithMany(u => u.Bookings)

                .HasForeignKey(b => b.UserId)

                .HasConstraintName("FK_Booking_Registration");

            entity.HasOne(b => b.ServiceCenter)

                .WithMany(sc => sc.Bookings)

                .HasForeignKey(b => b.ServiceCenterId)

                .HasConstraintName("FK_Booking_ServiceCenter");

            entity.HasOne(b => b.ServiceType)

                .WithMany(st => st.Bookings)

                .HasForeignKey(b => b.ServiceTypeId)

                .HasConstraintName("FK_Booking_ServiceType");

            entity.HasOne(b => b.Vehicle)

                .WithMany(v => v.Bookings)

                .HasForeignKey(b => b.VehicleId)

                .HasConstraintName("FK_Booking_Vehicle1");

            entity.HasOne(b => b.Mechanic)

            .WithMany()

            .HasForeignKey(b => b.MechanicId)

            .HasConstraintName("Fk_Booking_Mechanic");

            //.OneDelete(DeleteBehavior.SetNull);

        });

        // Invoice

        modelBuilder.Entity<Invoice>(entity =>

        {

            entity.HasKey(e => e.InvoiceId).HasName("PK__Invoice__D796AAB53CBEA5AA");

            entity.ToTable("Invoice");

            entity.Property(e => e.PaymentStatus)

                  .HasMaxLength(40)

                  .IsUnicode(false);
            entity.Property(e => e.Date)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

            entity.Property(e => e.TotalAmount)

                  .HasColumnType("decimal(10, 2)");

            entity.HasOne(i => i.Booking)

                .WithOne(b => b.Invoice)

                .HasForeignKey<Invoice>(i => i.BookingId)

                .HasConstraintName("FK_Invoice_Booking");

            entity.HasOne(i => i.ServiceType)

                .WithMany(st => st.Invoices)

                .HasForeignKey(i => i.ServiceTypeId)

                .HasConstraintName("FK_Invoice_ServiceType");

        });


        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}