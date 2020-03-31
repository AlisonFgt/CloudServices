using CloudServices.Common;
using System;

namespace CloudServices.Services.Queue
{
    public static class QueueTest
    {
        public static void Start()
        {
            string _queueName = AppSettingsHelper.GetConfig("QueueName");

            Console.WriteLine("Start Queue Tests Azure - Service Bus || AWS - SQS");
            var queueService = QueueServiceFactory.Create();
            queueService.SendMessage($"Hello cloud { DateTime.Now.ToString() }");
            Console.WriteLine("Send Message!");
            var msg = queueService.GetMessage(_queueName);
            Console.WriteLine("Get Message!");
            Console.WriteLine(msg.Body);
            queueService.DeleteMessage(_queueName, msg.ReceiptId);
            Console.WriteLine("Deleted Message!");
        }
    }
}
