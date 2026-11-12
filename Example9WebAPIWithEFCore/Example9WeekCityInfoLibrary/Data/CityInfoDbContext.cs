using Microsoft.EntityFrameworkCore;
using Example9WeekCityInfoLibrary.Models;

namespace Example9WeekCityInfoLibrary.Data
{
    public class CityInfoDbContext : DbContext
    {
        public CityInfoDbContext(DbContextOptions<CityInfoDbContext> options)
            : base(options)
        {
        }

        public DbSet<CityInfo> Cities { get; set; }
        public DbSet<PointsOfInterest> PointsOfInterest { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure CityInfo entity
            modelBuilder.Entity<CityInfo>(entity =>
            {
                entity.HasKey(e => e.CityId);

                entity.Property(e => e.CityName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Description)
                    .HasMaxLength(500);

                entity.HasMany(e => e.PointsOfInterest)
                    .WithOne(p => p.City)
                    .HasForeignKey(p => p.CityId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure PointsOfInterest entity
            modelBuilder.Entity<PointsOfInterest>(entity =>
            {
                entity.HasKey(e => e.PointsOfInterestId);

                entity.Property(e => e.NameofPOI)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Description)
                    .HasMaxLength(1000);

                entity.HasOne(e => e.City)
                    .WithMany(c => c.PointsOfInterest)
                    .HasForeignKey(e => e.CityId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
