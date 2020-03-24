using CloudServices.Common;
using CloudServices.Services.DocumentDB;
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
                var storageService = null;
                var documentService = DocumentDBServiceFactory.Create();
            }
            else
                Console.WriteLine("Bad AppSettings.json");
        }
    }
}
