using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace CheckInService.Models
{
    public class Flight
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("ExternalId")]
        [BsonRequired]
        public int ExternalId { get; set; }

        [BsonElement("FlightNumber")]
        [BsonRequired]
        public string FlightNumber { get; set; }

        [BsonElement("AirportDeparture")]
        [BsonRequired]
        public string AirportDeparture { get; set; }

        [BsonElement("AirportArrival")]
        [BsonRequired]
        public string AirportArrival { get; set; }

        [BsonElement("TimeDeparture")]
        [BsonRequired]
        public DateTime TimeDeparture { get; set; }

        [BsonElement("TimeArrival")]
        [BsonRequired]
        public DateTime TimeArrival { get; set; }

        [BsonElement("SeatsTotal")]
        public int SeatsTotal { get; set; }
    }
}
