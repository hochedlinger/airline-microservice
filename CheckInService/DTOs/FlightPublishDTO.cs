using System;

namespace CheckInService.DTOs
{
    public class FlightPublishDTO : GenericEventDTO
    {
        public int Id { get; set; }
        public string FlightNumber { get; set; }
        public string AirportDeparture { get; set; }
        public string AirportArrival { get; set; }  
        public DateTime TimeDeparture { get; set; }
        public DateTime TimeArrival { get; set; }
        public int SeatsTotal { get; set; } 
    }
}