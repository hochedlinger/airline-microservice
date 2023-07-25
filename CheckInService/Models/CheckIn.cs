using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CheckInService.Models
{
    public class CheckIn
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("bookingId")]
        [BsonRequired]
        public int BookingId { get; set; }
        [BsonElement("flightId")]
        [BsonRequired]
        public int FlightId { get; set; }
        [BsonElement("passengerName")]
        [BsonRequired]
        public string PassengerName { get; set; }
        [BsonElement("seatNumber")]
        public int SeatNumber { get; set; } 
        [BsonElement("checkInTime")]
        public DateTime CheckInTime { get; set; }
    }
}