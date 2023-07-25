using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingService.Models
{
    public class Flight
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int ExternalId { get; set; }
        [Required]
        public string FlightNumber { get; set; }
        [Required]
        public string AirportDeparture { get; set; }
        [Required]
        public string AirportArrival { get; set; }
        [Required]
        [Column(TypeName = "timestamp")]
        public DateTime TimeDeparture { get; set; }
        [Required]
        [Column(TypeName = "timestamp")]
        public DateTime TimeArrival { get; set; }
        [Required]
        public int SeatsTotal { get; set; } 
        public int SeatsBooked { get; set; }
        public decimal Price { get; set; }

        public ICollection<Booking> Bookings { get; set; }
    }
}