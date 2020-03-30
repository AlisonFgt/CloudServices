﻿using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace CloudServices.Services.DocumentDB
{
    public class MessageCosmosDB : TableEntity, IMessageDB
    {
        public MessageCosmosDB() { }

        public MessageCosmosDB(string partitionKey, string instance, string guid, string payload)
        {
            PartitionKey = partitionKey;
            Instance = instance;
            Guid = guid;
            Payload = payload;
            CreatedAt = DateTime.Now;
        }

        public string Instance { get; set; }

        public string Guid { get; set; }

        public string PartitionKey { get; set; }

        public string Payload { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
