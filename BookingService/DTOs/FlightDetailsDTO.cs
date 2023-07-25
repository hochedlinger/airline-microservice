using System;
using System.Collections.Generic;

namespace BookingService.DTOs
{
    public class FlightDetailsDTO
    {
        public int Id { get; set; }
        public string FlightNumber { get; set; }
        public ICollection<string> FlightNumberCodeShares { get; set; } = new List<string>();
        public string AirportDeparture { get; set; }
        public string AirportArrival { get; set; }  
        public DateTime TimeDeparture { get; set; }
        public DateTime TimeArrival { get; set; }
        public int SeatsTotal { get; set; } 
        public decimal Price { get; set; }
    }
}