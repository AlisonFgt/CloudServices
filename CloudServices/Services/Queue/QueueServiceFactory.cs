using CloudServices.Common;

namespace CloudServices.Services.Queue
{
    public static class QueueServiceFactory
    {
        public static IQueueService Create()
        {
            string cloud = AppSettingsHelper.GetCloud();

            switch (cloud)
            {
                case "azure":
                    return new AzureServiceBus();

                default:
                    return new AmazonSimpleQueue();
            }
        }
    }
}
