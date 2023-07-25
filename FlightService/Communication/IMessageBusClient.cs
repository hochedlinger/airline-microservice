using FlightService.DTOs;

namespace FlightService.Communication
{
    public interface IMessageBusClient
    {
        public void Publish(FlightPublishDTO flightPublishDTO);
    }
}