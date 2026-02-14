using Microsoft.EntityFrameworkCore;
using RdsVentures.Api.Models;

namespace RdsVentures.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Client> Clients { get; set; }
    public DbSet<Property> Properties { get; set; }
    public DbSet<Technician> Technicians { get; set; }
    public DbSet<ServiceRequest> ServiceRequests { get; set; }
    public DbSet<TimeEntry> TimeEntries { get; set; }
    public DbSet<Attachment> Attachments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Client configuration
        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ContactEmail).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(20);
        });

        // Property configuration
        modelBuilder.Entity<Property>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Address).IsRequired().HasMaxLength(300);
            entity.Property(e => e.City).IsRequired().HasMaxLength(100);
            entity.Property(e => e.State).IsRequired().HasMaxLength(2);
            entity.Property(e => e.Zip).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Notes).HasMaxLength(1000);

            entity.HasOne(e => e.Client)
                .WithMany(c => c.Properties)
                .HasForeignKey(e => e.ClientId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Technician configuration
        modelBuilder.Entity<Technician>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.HourlyRate).HasPrecision(18, 2);
        });

        // ServiceRequest configuration
        modelBuilder.Entity<ServiceRequest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(300);
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.Priority).HasConversion<int>();
            entity.Property(e => e.Status).HasConversion<int>();

            entity.HasOne(e => e.Property)
                .WithMany(p => p.ServiceRequests)
                .HasForeignKey(e => e.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.AssignedTech)
                .WithMany(t => t.ServiceRequests)
                .HasForeignKey(e => e.AssignedTechId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // TimeEntry configuration
        modelBuilder.Entity<TimeEntry>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Technician)
                .WithMany(t => t.TimeEntries)
                .HasForeignKey(e => e.TechId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ServiceRequest)
                .WithMany(sr => sr.TimeEntries)
                .HasForeignKey(e => e.ServiceRequestId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Attachment configuration
        modelBuilder.Entity<Attachment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BlobUrl).IsRequired().HasMaxLength(500);
            entity.Property(e => e.ContentType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Caption).HasMaxLength(500);

            entity.HasOne(e => e.ServiceRequest)
                .WithMany(sr => sr.Attachments)
                .HasForeignKey(e => e.ServiceRequestId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Technician)
                .WithMany(t => t.Attachments)
                .HasForeignKey(e => e.TechId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
