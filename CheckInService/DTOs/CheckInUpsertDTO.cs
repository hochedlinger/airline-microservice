namespace CheckInService.DTOs
{
    public class CheckInUpsertDTO
    {
        public int BookingId { get; set; }
        public int FlightId { get; set; }
        public string PassengerName { get; set; }
        public int SeatNumber { get; set; } 
    }
}