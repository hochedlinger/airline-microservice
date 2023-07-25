using System;
using System.Collections.Generic;
using System.Text.Json;
using CheckInService.Models;
using MongoDB.Driver;

namespace CheckInService.Data.Repos
{
    public class CheckInRepo : ICheckInRepo
    {
        private readonly MongoDbContext _ctx;

        public CheckInRepo(MongoDbContext ctx)
        {
            _ctx = ctx;
        }

        public IEnumerable<CheckIn> GetAllCheckIns()
        {
            return _ctx.CheckIns.Find(checkIns => true).ToList();
        }

        public IEnumerable<CheckIn> GetAllCheckInsByBookingId(int bookingId)
        {
            return _ctx.CheckIns.Find(c => c.BookingId == bookingId).ToList();
        }

        public IEnumerable<CheckIn> GetAllCheckInsByFlightId(int flightId)
        {
            return _ctx.CheckIns.Find(c => c.FlightId == flightId).ToList();
        }

        public CheckIn GetCheckInById(string id)
        {
            return _ctx.CheckIns.Find(c => c.Id == id).FirstOrDefault();
        }

        public void CreateCheckIn(CheckIn checkIn)
        {
            var flight = _ctx.Flights.Find(f => f.ExternalId == checkIn.FlightId).FirstOrDefault();
            if (flight == null) 
            {
                throw new InvalidOperationException($"Flight {checkIn.FlightId} not found");
            }

            var booking = _ctx.Bookings.Find(b => b.ExternalId == checkIn.BookingId).FirstOrDefault();
            if (booking == null) 
            {
                throw new InvalidOperationException($"Booking {checkIn.BookingId} not found");
            }

            var seats = _ctx.Flights.Find(f => f.ExternalId == checkIn.FlightId).FirstOrDefault().SeatsTotal;
            if (checkIn.SeatNumber <= 0 || checkIn.SeatNumber > seats)
            {
                throw new InvalidOperationException($"Seat number {checkIn.SeatNumber} doesn't exist on this flight");
            }

            var existingCheckIn = _ctx.CheckIns.Find(c => c.FlightId == checkIn.FlightId && c.SeatNumber == checkIn.SeatNumber).FirstOrDefault();
            if (existingCheckIn != null)
            {
                throw new InvalidOperationException("Seat number is already taken");
            }

            _ctx.CheckIns.InsertOne(checkIn);
        }

        public void UpdateCheckIn(CheckIn checkIn)
        {
            var flight = _ctx.Flights.Find(f => f.ExternalId == checkIn.FlightId).FirstOrDefault();
            if (flight == null) 
            {
                throw new InvalidOperationException($"Flight {checkIn.FlightId} not found");
            }

            var booking = _ctx.Bookings.Find(b => b.ExternalId == checkIn.BookingId).FirstOrDefault();
            if (booking == null) 
            {
                throw new InvalidOperationException($"Booking {checkIn.BookingId} not found");
            }

            var seats = _ctx.Flights.Find(f => f.ExternalId == checkIn.FlightId).FirstOrDefault().SeatsTotal;
            if (checkIn.SeatNumber <= 0 || checkIn.SeatNumber > seats)
            {
                throw new InvalidOperationException($"Seat number {checkIn.SeatNumber} doesn't exist on this flight");
            }

            var existingCheckIn = _ctx.CheckIns.Find(c => c.FlightId == checkIn.FlightId && c.SeatNumber == checkIn.SeatNumber).FirstOrDefault();
            if (existingCheckIn != null && existingCheckIn.Id != checkIn.Id)
            {
                throw new InvalidOperationException("Seat number is already taken");
            }

            _ctx.CheckIns.ReplaceOne(c => c.Id == checkIn.Id, checkIn);
        }



        public bool FlightExist(int flightId)
        {
            return _ctx.Flights.Find(c => c.ExternalId == flightId).Any();
        }

        public void CreateFlight(Flight flight)
        {
            _ctx.Flights.InsertOne(flight);
        }

        public void UpdateFlight(Flight flight)
        {
            _ctx.Flights.ReplaceOne(f => f.ExternalId == flight.ExternalId, flight);
        }


        public bool BookingExist(int bookingId)
        {
            return _ctx.Bookings.Find(b => b.ExternalId == bookingId).Any();
        }

        public void CreateBooking(Booking booking)
        {
            _ctx.Bookings.InsertOne(booking);
        }

        public void UpdateBooking(Booking booking)
        {
            _ctx.Bookings.ReplaceOne(b => b.ExternalId == booking.ExternalId, booking);
        }
    }
}