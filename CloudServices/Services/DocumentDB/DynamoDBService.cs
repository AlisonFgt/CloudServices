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

        public dynamic GetItem(string partitionKey, string rowKey)
        {
            try
            {
                var guid = new Guid(rowKey);
                return table.GetItem(partitionKey, guid);
            }
            catch (Exception ex)
            {
                Console.WriteLine("DynamoDBService - GetItem", ex?.Message);
                return null;
            }
        }

        public dynamic PutItem(object document)
        {
            try
            {
                if (document != null && document is Document)
                {
                    table.PutItem(document as Document);
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
    }
}
