using PetsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace PetsApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Pet> Pets { get; set; }  
        public DbSet<Sighting> Sightings { get; set; }  

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Sighting>()
                .HasOne(s => s.Pet)
                .WithMany(p => p.Sightings)
                .HasForeignKey(s => s.PetId)
                .OnDelete(DeleteBehavior.Cascade);

            // Optional indexes for performance
            modelBuilder.Entity<Pet>()
                .HasIndex(p => new { p.Latitude, p.Longitude });

            modelBuilder.Entity<Sighting>()
                .HasIndex(s => new { s.Latitude, s.Longitude });

            base.OnModelCreating(modelBuilder);

            
        }

    }
}
