namespace BookingService.Communication.EventProcessor
{
    public interface IEventProcessor
    {
        void Process(string msg);
    }
}