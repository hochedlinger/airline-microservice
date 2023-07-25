namespace CheckInService.Communication.EventProcessor
{
    public interface IEventProcessor
    {
        void Process(string msg);
    }
}