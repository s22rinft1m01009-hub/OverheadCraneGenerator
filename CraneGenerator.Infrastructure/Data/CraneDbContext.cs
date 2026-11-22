using CraneGenerator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CraneGenerator.Infrastructure.Data
{
    public class CraneDbContext : DbContext
    {
        public CraneDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<CraneDesign> CraneDesigns { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CraneDesign>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.DesignName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(x => x.CraneType)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(x => x.CapacityTonnes)
                    .IsRequired();

                entity.Property(x => x.SpanMm)
                    .IsRequired();

                entity.Property(x => x.LiftHeightMm)
                    .IsRequired();

                entity.Property(x => x.RunwayWidthMm)
                    .IsRequired();

                entity.Property(x => x.HoistPositionMm)
                    .IsRequired();

                entity.Property(x => x.BuildingHeightMm)
                    .IsRequired();

                entity.Property(x => x.CreatedAt)
                    .IsRequired();

                entity.Property(x => x.UpdatedAt)
                    .IsRequired();
            });
        }
    }
}
