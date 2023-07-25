using System.Collections.Generic;
using CheckInService.Models;

namespace CheckInService.Communication.Grpc
{
    public interface IBookingClient
    {
        IEnumerable<Booking> GetAllBookings();
    }
}