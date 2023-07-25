using System;

namespace BookingService.DTOs
{
    public class BookingUpsertDTO
    {
        public int FlightId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public int NumberOfPassengers { get; set; }
    }
}