using CloudServices.Common;
using CloudServices.Services.DocumentDB;
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
                var storageService = 1;
                var documentService = DocumentDBServiceFactory.Create();
                var queueService = QueueServiceFactory.Create();
                queueService.SendMessage("olá to na azure 123");
            }
            else
                Console.WriteLine("Bad AppSettings.json");
        }
    }
}
