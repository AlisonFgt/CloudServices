using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using CloudServices.Common;
using System;

namespace CloudServices.Services.DocumentDB
{
    public class DynamoDBService : IDocumentDBService
    {
        private static readonly string DynamoTableName = AppSettingsHelper.GetConfig("DocumentDBTable");

        private static Table table;

        public DynamoDBService()
        {
            try
            {
                var clientConfig = new AmazonDynamoDBConfig()
                {
                    RegionEndpoint = RegionEndpoint.SAEast1
                };

                var client = new AmazonDynamoDBClient(clientConfig);
                table = Table.LoadTable(client, DynamoTableName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("DynamoDBService - LoadTable", ex?.Message);
            }
        }

        public IMessageDB GetItem(string partitionKey, string rowKey)
        {
            try
            {
                var guid = new Guid(rowKey);
                var document = table.GetItem(partitionKey, guid);
                return IMessageToDocument(document);
            }
            catch (Exception ex)
            {
                Console.WriteLine("DynamoDBService - GetItem", ex?.Message);
                return null;
            }
        }

        public dynamic PutItem(IMessageDB document)
        {
            try
            {
                if (document != null && document is MessageDynamoDB)
                {
                    Document item = IMessageToDocument(document);
                    table.PutItem(item);
                    return table;
                }
                else
                {
                    Console.WriteLine("PutItem DynamoDB - Document is null Or Document is not type Document");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("PutItem DynamoDB", ex?.Message);
            }

            return null;
        }

        private static Document IMessageToDocument(IMessageDB document)
        {
            var item = new Document();

            item["PartitionKey"] = document.PartitionKey;
            item["Guid"] = document.Guid;
            item["CreatedAt"] = document.CreatedAt;
            item["Instance"] = document.Instance;
            item["Payload"] = document.Payload;
            return item;
        }

        private static IMessageDB IMessageToDocument(Document document)
        {
            var item = new MessageDynamoDB();

            item.PartitionKey = document["PartitionKey"];
            item.Guid = document["Guid"];
            item.Instance = document["Instance"];
            item.Payload = document["Payload"];
            item.CreatedAt = DateTime.Parse(document["CreatedAt"]);
            return item;
        }

        public IMessageDB DeleteItem(string partitionKey, string rowKey)
        {
            try
            {
                var guid = new Guid(rowKey);
                var document = table.DeleteItem(partitionKey, guid);
                return IMessageToDocument(document);
            }
            catch (Exception ex)
            {
                Console.WriteLine("DynamoDBService - DeleteItem", ex?.Message);
                return null;
            }
        }
    }
}
