using System;

namespace BookingService.DTOs
{
    public class BookingSummaryDTO
    {
        public int Id { get; set; }
        public int FlightId { get; set; }
        public string CustomerName { get; set; }
        public DateTime BookingDate { get; set; }
    }
}