
namespace CloudServices.Services.Queue
{
    public interface IQueueService
    {
        bool SendMessage(string message);

        Model.Message GetMessage(string queue);

        bool DeleteMessage(string queue, string receiptHandle = "");
    }
}
