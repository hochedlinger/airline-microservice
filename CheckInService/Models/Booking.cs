using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CheckInService.Models
{
    public class Booking
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("externalId")]
        public int ExternalId { get; set; }

        [BsonElement("flightId")]
        public int FlightId { get; set; }

        [BsonElement("customerName")]
        public string CustomerName { get; set; }

        [BsonElement("numberOfPassengers")]
        public int NumberOfPassengers { get; set; }
    }

}