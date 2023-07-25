using System.Collections.Generic;
using CheckInService.Models;

namespace CheckInService.Data.Repos
{
    public interface ICheckInRepo
    {
        IEnumerable<CheckIn> GetAllCheckIns();
        IEnumerable<CheckIn> GetAllCheckInsByFlightId(int flightId);
        IEnumerable<CheckIn> GetAllCheckInsByBookingId(int bookingId);
        CheckIn GetCheckInById(string id);
        void CreateCheckIn(CheckIn checkIn);
        void UpdateCheckIn(CheckIn checkIn);

        public bool FlightExist(int flightId);
        public void CreateFlight(Flight flight);
        public void UpdateFlight(Flight flight);

        public bool BookingExist(int bookingId);
        public void CreateBooking(Booking booking);
        public void UpdateBooking(Booking booking);
    }
}