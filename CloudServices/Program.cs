using CloudServices.Common;
using CloudServices.Services.Queue;
using CloudServices.Services.Storage;
using System;
using System.IO;

namespace CloudServices
{
    class Program
    {
        static void Main(string[] args)
        {
            //var teste = new UploadObjectTest();

            string cloud = AppSettingsHelper.GetCloud();

            if (!string.IsNullOrEmpty(cloud))
            {
                Console.WriteLine($"Start Service in Cloud : { cloud }");
                StorageTest(cloud);
                Console.WriteLine("Finishing Tests");
            }
            else
                Console.WriteLine("Bad AppSettings.json");
        }

        private static void StorageTest(string cloud)
        {
            Console.WriteLine("Start Storage Tests Azure - Blob Storage || AWS - S3");
            var pathProject = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\.."));
            var storageService = StorageServiceFactory.Create();
            var fileName = "FileUploadByPath_S3.jpg";
            var filePath = Path.Combine(pathProject, "FilesTests/logo_trade.png");
            var containerName = "tradeforce.instances/hlg/brf/Tests/";


            if (cloud.Equals("azure"))
            {
                storageService.UploadByPath(containerName, fileName, filePath);
                Console.WriteLine("Upload file " + fileName);
                Console.WriteLine("Url = " + storageService.GetUrl(containerName, fileName));
            }
            else
            {

            }
        }

        private static void QueueTest()
        {
            Console.WriteLine("Start Queue Tests Azure - Service Bus || AWS - SQS");
            var queueService = QueueServiceFactory.Create();
            queueService.SendMessage($"Hello cloud { DateTime.Now.ToString() }");
        }
    }
}
