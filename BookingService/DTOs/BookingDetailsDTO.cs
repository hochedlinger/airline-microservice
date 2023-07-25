using System;

namespace BookingService.DTOs
{
    public class BookingDetailsDTO
    {
        public int Id { get; set; }
        public int FlightId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public int NumberOfPassengers { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime BookingDate { get; set; }
    }
}