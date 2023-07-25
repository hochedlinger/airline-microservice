using Microsoft.EntityFrameworkCore;
using FlightService.Models;

namespace FlightService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }

        public DbSet<Flight> Flights { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Flight>()
                .Property(f => f.Price)
                .HasPrecision(8, 2);

            modelBuilder.Entity<FlightNumberCodeShare>()
                .HasOne(fcs => fcs.Flight)
                .WithMany(f => f.FlightNumberCodeShares)
                .HasForeignKey(fcs => fcs.FlightId);
        }
    }
}