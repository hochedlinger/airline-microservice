using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FlightService.DTOs
{
    public class FlightUpsertDTO
    {
        [Required]
        public string FlightNumber { get; set; }
        public ICollection<string> FlightNumberCodeShares { get; set; } = new List<string>();
        [Required]
        public string AirportDeparture { get; set; }
        [Required]
        public string AirportArrival { get; set; }  
        [Required]
        public DateTime TimeDeparture { get; set; }
        [Required]
        public DateTime TimeArrival { get; set; }
        [Required]
        public int SeatsTotal { get; set; } 
        [Required]
        public decimal Price { get; set; }
    }
}