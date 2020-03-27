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

        public object GetItem(string partitionKey, Guid documentId)
        {
            try
            {
                return table.GetItem(partitionKey, documentId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("DynamoDBService - GetItem", ex?.Message);
                return null;
            }
        }

        public object PutItem(object document)
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
