using Microsoft.EntityFrameworkCore;
using BookingService.Models;

namespace BookingService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }
 
        public DbSet<Flight> Flights { get; set; }
        public DbSet<Booking> Bookings { get; set; }
    }
}