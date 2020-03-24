namespace CloudServices.Services.Queue
{
    public interface IQueueService
    {
        bool SendMessage(string message);
    }
}
