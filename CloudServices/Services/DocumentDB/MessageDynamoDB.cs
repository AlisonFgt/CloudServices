using Amazon.DynamoDBv2.DocumentModel;
using System;

namespace CloudServices.Services.DocumentDB
{
    public class MessageDynamoDB : Document, IMessageDB
    {
        public MessageDynamoDB() { }

        public MessageDynamoDB(string partitionKey, string instance, string guid, string payload)
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
