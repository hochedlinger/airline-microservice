using System;

namespace FlightService.DTOs
{
    public class FlightSummaryDTO
    {
        public int Id { get; set; }
        public string FlightNumber { get; set; }
        public string AirportDeparture { get; set; }
        public string AirportArrival { get; set; }  
        public DateTime TimeDeparture { get; set; }
        public DateTime TimeArrival { get; set; }
        public decimal Price { get; set; }
    }
}