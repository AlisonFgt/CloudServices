using CloudServices.Common;
using CloudServices.Services.DocumentDB;
using CloudServices.Services.Queue;
using CloudServices.Services.Storage;
using System;

namespace CloudServices
{
    class Program
    {
        static void Main(string[] args)
        {
            string cloud = AppSettingsHelper.GetCloud();

            if (!string.IsNullOrEmpty(cloud))
            {
                Console.WriteLine($"Starting testing cloud services in { cloud }");
                Console.WriteLine(DateTime.Now.ToString());
                StorageTest.Start();
                //QueueTest.Start();
                //DocumentDBTest.Start();
                Console.WriteLine(DateTime.Now.ToString());
                Console.WriteLine("Finishing Tests");
            }
            else
                Console.WriteLine("Bad AppSettings.json");
        }
    }
}
