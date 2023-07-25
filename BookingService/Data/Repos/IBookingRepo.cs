using System.Collections.Generic;
using System.Data;
using BookingService.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace BookingService.Data.Repos
{
    public interface IBookingRepo
    {
        IEnumerable<Booking> GetAllBookings();
        IEnumerable<Booking> GetBookingsByFlightId(int flightId);
        Booking GetBookingById(int id);
        void CreateBooking(Booking booking);
        void UpdateBooking(Booking booking);

        bool FlightExist(int externalFlightId);
        void CreateFlight(Flight flight);

        int SaveChanges();
    }
}