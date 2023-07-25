namespace CheckInService.DTOs
{
    public class BookingPublishDTO : GenericEventDTO
    {
        public int Id { get; set; }   
        public int FlightId { get; set; }
        public string CustomerName { get; set; }
        public int NumberOfPassengers { get; set; }
    }
}