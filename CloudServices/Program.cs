using CloudServices.Common;
using CloudServices.Services.Queue;
using System;

namespace CloudServices
{
    class Program
    {
        static void Main(string[] args)
        {
            var cloud = AppSettingsHelper.GetCloud();

            if (!string.IsNullOrEmpty(cloud))
            {
                Console.WriteLine($"Start Service in Cloud : { cloud }");
                QueueTest();
                Console.WriteLine("Finishing Tests");
            }
            else
                Console.WriteLine("Bad AppSettings.json");
        }

        private static void QueueTest()
        {
            Console.WriteLine("Start Queue Tests Azure - Service || AWS - SQS");
            var queueService = QueueServiceFactory.Create();
            queueService.SendMessage($"Hello cloud { DateTime.Now.ToString() }");
        }
    }
}
