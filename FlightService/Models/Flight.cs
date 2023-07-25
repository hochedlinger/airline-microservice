using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FlightService.Models
{
    public class Flight
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string FlightNumber { get; set; }
        public ICollection<FlightNumberCodeShare> FlightNumberCodeShares { get; set; } = new List<FlightNumberCodeShare>();
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