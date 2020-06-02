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

            item["partitionKey"] = document.PartitionKey;
            item["guid"] = document.Guid;
            item["createdAt"] = document.CreatedAt;
            item["instance"] = document.Instance;
            item["payload"] = document.Payload;
            return item;
        }

        private static IMessageDB IMessageToDocument(Document document)
        {
            var item = new MessageDynamoDB();

            item.PartitionKey = document["partitionKey"];
            item.Guid = document["guid"];
            item.Instance = document["instance"];
            item.Payload = document["payload"];
            item.CreatedAt = DateTime.Parse(document["createdAt"]);
            return item;
        }

        public IMessageDB DeleteItem(string partitionKey, string rowKey)
        {
            try
            {
                var guid = new Guid(rowKey);
                table.DeleteItem(partitionKey, guid);
                return new MessageDynamoDB();
            }
            catch (Exception ex)
            {
                Console.WriteLine("DynamoDBService - DeleteItem", ex?.Message);
                return null;
            }
        }
    }
}
