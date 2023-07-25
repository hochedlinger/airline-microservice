using System;

namespace FlightService.DTOs
{
    public class FlightPublishDTO
    {
        public int Id { get; set; }
        public string FlightNumber { get; set; }
        public string AirportDeparture { get; set; }
        public string AirportArrival { get; set; }  
        public DateTime TimeDeparture { get; set; }
        public DateTime TimeArrival { get; set; }
        public int SeatsTotal { get; set; } 
        public decimal Price { get; set; }
        public string Event { get; set; }
    }
}