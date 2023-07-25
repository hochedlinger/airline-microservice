using CheckInService.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CheckInService.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<DatabaseSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.Database);
        }
        public IMongoCollection<Flight> Flights => _database.GetCollection<Flight>("flights");
        public IMongoCollection<Booking> Bookings => _database.GetCollection<Booking>("bookings");
        public IMongoCollection<CheckIn> CheckIns => _database.GetCollection<CheckIn>("checkins");
    }
}