using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using BookingService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BookingService.Data.Repos
{
    public class BookingRepo : IBookingRepo
    {
        private readonly AppDbContext _ctx;

        public BookingRepo(AppDbContext ctx)
        {
            _ctx = ctx;
        }        

        public IEnumerable<Booking> GetAllBookings()
        {
            return _ctx.Bookings.ToList();
        }

        public IEnumerable<Booking> GetBookingsByFlightId(int flightId)
        {
            return _ctx.Bookings.Where(b => b.FlightId == flightId);
        }

        public Booking GetBookingById(int id)
        {
            return _ctx.Bookings
                .Include(b => b.Flight)
                .FirstOrDefault(b => b.Id == id);
        }

        public void CreateBooking(Booking booking)
        {
            if (booking == null) { throw new ArgumentNullException(nameof(booking)); }

            var flight = _ctx.Flights.FirstOrDefault(f => f.ExternalId == booking.FlightId);
            if (flight == null) 
            {
                throw new InvalidOperationException($"Flight {booking.FlightId} not found");
            }

            var availableSeats = flight.SeatsTotal - flight.SeatsBooked;
            if (availableSeats < booking.NumberOfPassengers)
            {
                throw new InvalidOperationException($"Flight {booking.FlightId} has {availableSeats} available seat{(availableSeats == 1 ? "" : "s")}");
            }

            flight.SeatsBooked += booking.NumberOfPassengers;
            _ctx.Bookings.Add(booking);
            _ctx.SaveChanges();
        }

        public void UpdateBooking(Booking booking)
        {
            if (booking == null) { throw new ArgumentNullException(nameof(booking)); }

            var originalBooking = _ctx.Bookings.AsNoTracking().FirstOrDefault(b => b.Id == booking.Id);
            if (originalBooking != null)
            {
                int passengerDifference = booking.NumberOfPassengers - originalBooking.NumberOfPassengers;

                Console.WriteLine($"PassengerDifference: {passengerDifference}");

                var flight = _ctx.Flights.FirstOrDefault(f => f.ExternalId == booking.FlightId);
                if (flight != null)
                {
                    var availableSeats = flight.SeatsTotal - flight.SeatsBooked;
                    if (availableSeats < passengerDifference)
                    {
                        throw new InvalidOperationException($"Flight {booking.FlightId} has {availableSeats} available seat{(availableSeats == 1 ? "" : "s")}");
                    }

                    flight.SeatsBooked += passengerDifference;    
                    _ctx.Bookings.Update(booking);
                }
            }
        }



        public bool FlightExist(int externalFlightId)
        {
            return _ctx.Flights.Any(f => f.ExternalId == externalFlightId);
        }

        public void CreateFlight(Flight flight)
        {
             if (flight == null)
            {
                throw new ArgumentNullException(nameof(flight)); 
            }
            _ctx.Flights.Add(flight);
        }


        public int SaveChanges()
        {
            return _ctx.SaveChanges();
        }
    }
}