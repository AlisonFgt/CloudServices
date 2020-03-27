using CloudServices.Common;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using System;
using System.Text;

namespace CloudServices.Services.Queue
{
    public class AzureServiceBus : IQueueService
    {
        string ServiceBusConnectionString = AppSettingsHelper.GetConfig("QueueConnectionString");

        string QueueName = AppSettingsHelper.GetConfig("QueueName");

        static IQueueClient queueClient;

        private QueueClient GetClient()
        {
            return new QueueClient(ServiceBusConnectionString, QueueName);
        }

        public bool SendMessage(string message)
        {
            try
            {
                var msg = new Microsoft.Azure.ServiceBus.Message(Encoding.UTF8.GetBytes(message));
                queueClient = GetClient();
                queueClient.SendAsync(msg).Wait();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("AzureServiceBus - SendMessage - " + ex?.Message);
            }

            return false;
        }


        public Model.Message GetMessage(string queue)
        {
            try
            {
                var messageReceiver = new MessageReceiver(ServiceBusConnectionString, queue, ReceiveMode.PeekLock);
                var message = messageReceiver.ReceiveAsync().Result;
                
                // Process the message
                Console.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");

                // Complete the message so that it is not received again.
                messageReceiver.CompleteAsync(message.SystemProperties.LockToken).Wait();
                messageReceiver.CloseAsync().Wait();
                return new Model.Message
                {
                    Body = Encoding.UTF8.GetString(message.Body),
                    Queue = new Model.Queue { Name = queue },
                    MessageId = message.MessageId,
                    ReceiptId = message.SystemProperties.SequenceNumber.ToString()
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine("AzureServiceBus - GetMessage - " + ex?.Message);
                return null;
            }
        }

        public bool DeleteMessage(string queue)
        {
            try
            {
                var messageReceiver = new MessageReceiver(ServiceBusConnectionString, queue, ReceiveMode.PeekLock);
                var message = messageReceiver.ReceiveAsync().Result;

                // Complete the message so that it is not received again.
                messageReceiver.CompleteAsync(message.SystemProperties.LockToken).Wait();
                messageReceiver.CloseAsync().Wait();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("AzureServiceBus - GetMessage - " + ex?.Message);
                return false;
            }
        }
    }
}
