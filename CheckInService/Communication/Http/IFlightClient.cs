using System.Collections.Generic;
using System.Threading.Tasks;
using CheckInService.DTOs;

namespace CheckInService.Communication.Http
{
    public interface IFlightClient
    {
        Task<IEnumerable<int>> GetAllFlightIds();
        Task<FlightDetailsDTO> GetFlightById(int id);
    }
}