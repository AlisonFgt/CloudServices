using System;
using System.IO;
using System.Text;

namespace CloudServices.Services.Storage
{
    public static class StorageTest
    {
        public static void Start()
        {
            Console.WriteLine("Starting testing in Storage Service [Azure - Blob Storage | AWS - S3]");
            var pathProject = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\.."));
            var storageService = StorageServiceFactory.Create();
            var fileName = "FileUploadByPath.png";
            var fileNametwo = "FileUploadByPath_two.png";
            var filePath = Path.Combine(pathProject, "FilesTests/logo_trade.png");
            var containerName = "tradeforce.instances/hlg/brf/Tests/";
            var msg = "Agora estou na AWS & Azure";

            storageService.UploadByPath(containerName, fileName, filePath);
            Console.WriteLine("Upload file " + fileName);
            Console.WriteLine("Url = " + storageService.GetUrl(containerName, fileName));
            Console.WriteLine("Url expireDays = " + storageService.GetUrl(containerName, fileName, 30));
            var image = storageService.GetImage(containerName, fileName);
            storageService.UploadByImage(containerName, fileNametwo, image);
            Console.WriteLine("Upload file two" + fileNametwo);
            Console.WriteLine("Url file two = " + storageService.GetUrl(containerName, fileNametwo));
            MemoryStream msWrite = new MemoryStream(Encoding.UTF8.GetBytes(msg));
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
    }
}
