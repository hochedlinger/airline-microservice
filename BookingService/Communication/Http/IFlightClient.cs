using System.Collections.Generic;
using System.Threading.Tasks;
using BookingService.DTOs;

namespace BookingService.Communication.Http
{
    public interface IFlightClient
    {
        Task<IEnumerable<int>> GetAllFlightIds();
        Task<FlightDetailsDTO> GetFlightById(int id);
    }
}