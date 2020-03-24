using CloudServices.Common;
using Microsoft.Azure.ServiceBus;
using System;
using System.Text;

namespace CloudServices.Services.Queue
{
    public class AzureServiceBus : IQueueService
    {
        string ServiceBusConnectionString = AppSettingsHelper.GetConfig("QueueConnectionString");

        string QueueName = AppSettingsHelper.GetConfig("QueueName");

        static IQueueClient queueClient;

        public bool SendMessage(string message)
        {
            try
            {
                var msg = new Message(Encoding.UTF8.GetBytes(message));
                queueClient = new QueueClient(ServiceBusConnectionString, QueueName);
                queueClient.SendAsync(msg).Wait();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("AzureServiceBus - SendMessage - " + ex?.Message);
            }

            return false;
        }
    }
}
