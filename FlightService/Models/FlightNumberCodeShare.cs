using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlightService.Models
{
    public class FlightNumberCodeShare
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string CodeShare { get; set; }

        [Required]
        public int FlightId { get; set; }

        [ForeignKey("FlightId")]
        public Flight Flight { get; set; }
    }
}