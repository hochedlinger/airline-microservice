using BookingService.DTOs;

namespace BookingService.Communication
{
    public interface IMessageBusClient
    {
        public void Publish(BookingPublishDTO bookingPublishDTO);
    }
}