using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingService.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int FlightId { get; set; }
        public Flight Flight { get; set; }
        [Required]
        public string CustomerName { get; set; }
        [Required]
        public string CustomerEmail { get; set; }
        [Required]
        [Range(1, 10)]
        public int NumberOfPassengers { get; set; }
        [Column(TypeName = "timestamp")]
        public DateTime BookingDate { get; set; }
    }
}