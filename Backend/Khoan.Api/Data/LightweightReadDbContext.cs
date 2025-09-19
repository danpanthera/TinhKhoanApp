using Microsoft.EntityFrameworkCore;
using Khoan.Api.Models;

namespace Khoan.Api.Data
{
    /// <summary>
    /// Lightweight DbContext for simple read endpoints used by the frontend.
    /// Maps only ImportedDataRecords and Positions to avoid heavy model validation on temporal tables.
    /// </summary>
    public class LightweightReadDbContext : DbContext
    {
        public LightweightReadDbContext(DbContextOptions<LightweightReadDbContext> options) : base(options) { }

        public DbSet<ImportedDataRecord> ImportedDataRecords { get; set; }
        public DbSet<Position> Positions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Map tables explicitly and keep configuration minimal
            modelBuilder.Entity<ImportedDataRecord>(entity =>
            {
                entity.ToTable("ImportedDataRecords");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FileName).HasMaxLength(255).IsRequired();
                entity.Property(e => e.FileType).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Category).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Status).HasMaxLength(50).IsRequired();
                entity.Property(e => e.ImportedBy).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Notes).HasMaxLength(1000);
            });

            modelBuilder.Entity<Position>(entity =>
            {
                entity.ToTable("Positions");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(150).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(500);
                // Do not materialize navigation to Employees to avoid pulling in Employee entity graph
                entity.Ignore(p => p.Employees);
            });

            // Explicitly ignore unrelated entities so EF doesn't try to build them
            modelBuilder.Ignore<Employee>();
            modelBuilder.Ignore<Role>();
            modelBuilder.Ignore<EmployeeRole>();
        }
    }
}
