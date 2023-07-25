using System;
using System.Collections.Generic;
using System.Linq;
using FlightService.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightService.Data.Repos
{
    public class FlightRepo : IFlightRepo
    {
        private readonly AppDbContext _ctx;

        public FlightRepo(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public IEnumerable<Flight> GetAllFlights()
        {
            return _ctx.Flights.ToList();
        }

        public Flight GetFlightById(int id)
        {
            return _ctx.Flights
                .Include(f => f.FlightNumberCodeShares)
                .FirstOrDefault(f => f.Id == id);
        }

        public void CreateFlight(Flight flight)
        {
            if (flight == null) { throw new ArgumentNullException(nameof(flight)); }
            _ctx.Flights.Add(flight);
        }

        public void UpdateFlight(Flight flight)
        {
            if (flight == null) { throw new ArgumentNullException(nameof(flight)); }

            var flightToUpdate = _ctx.Flights.FirstOrDefault(f => f.Id == flight.Id);

            if (flightToUpdate != null)
            {
                flightToUpdate.AirportArrival = flight.AirportArrival;
                flightToUpdate.AirportDeparture = flight.AirportDeparture;
                flightToUpdate.TimeArrival = flight.TimeArrival;
                flightToUpdate.TimeDeparture = flight.TimeDeparture;
                flightToUpdate.SeatsTotal = flight.SeatsTotal;
                flightToUpdate.Price = flight.Price;

                _ctx.Flights.Update(flightToUpdate);
            }

        }

        public int SaveChanges()
        {
            return _ctx.SaveChanges();
        }
    }
}