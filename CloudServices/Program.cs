using CloudServices.Common;
using CloudServices.Services.DocumentDB;
using CloudServices.Services.Queue;
using CloudServices.Services.Storage;
using System;
using System.IO;
using System.Text;

namespace CloudServices
{
    class Program
    {
        static void Main(string[] args)
        {
            string cloud = AppSettingsHelper.GetCloud();

            if (!string.IsNullOrEmpty(cloud))
            {
                Console.WriteLine($"Start Service in Cloud : { cloud }");
                //StorageTest();
                //QueueTest();
                DocumentDBTest();
                Console.WriteLine("Finishing Tests");
            }
            else
                Console.WriteLine("Bad AppSettings.json");
        }

        private static void DocumentDBTest()
        {
            Console.WriteLine("Start Queue Tests Azure - Service Bus || AWS - SQS");
            var documentoDBService = DocumentDBServiceFactory.Create();
            documentoDBService.
            Console.WriteLine("Send Message!");
            Console.WriteLine("Get Message!");
            Console.WriteLine(msg.Body);
            Console.WriteLine("Deleted Message!");
        }

        private static void StorageTest()
        {
            Console.WriteLine("Start Storage Tests Azure - Blob Storage || AWS - S3");
            var pathProject = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\.."));
            var storageService = StorageServiceFactory.Create();
            var fileName = "FileUploadByPath_S3.png";
            var fileNametwo = "FileUploadByPath_S3_two.png";
            var filePath = Path.Combine(pathProject, "FilesTests/logo_trade.png");
            var containerName = "tradeforce.instances/hlg/brf/Tests/";

            storageService.UploadByPath(containerName, fileName, filePath);
            Console.WriteLine("Upload file " + fileName);
            Console.WriteLine("Url = " + storageService.GetUrl(containerName, fileName));
            Console.WriteLine("Url expireDays = " + storageService.GetUrl(containerName, fileName, 30));
            var image = storageService.GetImage(containerName, fileName);
            storageService.UploadByImage(containerName, fileNametwo, image);
            Console.WriteLine("Upload file two" + fileNametwo);
            Console.WriteLine("Url file two = " + storageService.GetUrl(containerName, fileNametwo));
            MemoryStream msWrite = new MemoryStream(Encoding.UTF8.GetBytes("Agora estou na AWS & Azure"));
            msWrite.Position = 0;
            using (msWrite)
            {
                storageService.UploadByStream(containerName, "File.txt", msWrite);
            }
            Console.WriteLine("UploadByStream to File.txt");
            storageService.Download(containerName, "File.txt", Path.Combine(pathProject, "FilesTests/File_2.txt"));
            Console.WriteLine("Download to File.txt");
            storageService.Delete(containerName, fileNametwo);
        }

        private static void QueueTest()
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
