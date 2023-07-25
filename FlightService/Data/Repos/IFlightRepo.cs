using System.Collections.Generic;
using FlightService.Models;

namespace FlightService.Data.Repos
{
    public interface IFlightRepo
    {
        IEnumerable<Flight> GetAllFlights();
        Flight GetFlightById(int id);
        void CreateFlight(Flight flight);
        void UpdateFlight(Flight flight);
        int SaveChanges();
    }
}