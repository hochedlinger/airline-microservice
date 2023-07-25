using System;

namespace CheckInService.DTOs
{
    public class CheckInDetailsDTO
    {
        public string Id { get; set; }
        public int BookingId { get; set; }
        public int FlightId { get; set; }
        public string PassengerName { get; set; }
        public int SeatNumber { get; set; } 
        public DateTime CheckInTime { get; set; }
    }
}