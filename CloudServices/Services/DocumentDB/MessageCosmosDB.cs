using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace CloudServices.Services.DocumentDB
{
    public class MessageCosmosDB : TableEntity
    {
        public MessageCosmosDB()
        {

        }

        public MessageCosmosDB(string partitionKey, string instance, string payload)
        {
            PartitionKey = partitionKey;
            RowKey = instance;
            Payload = payload;
            CreatedAt = DateTime.Now;
        }

        public DateTime CreatedAt { get; set; }

        public string Payload { get; set; }
    }
}
