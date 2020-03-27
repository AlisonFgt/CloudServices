using CloudServices.Common;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace CloudServices.Services.DocumentDB
{
    public class CosmosDBService : IDocumentDBService
    {
        private readonly string DocumentDBConnectionString = AppSettingsHelper.GetConfig("DocumentDBConnectionString");

        private static readonly string CosmosTableName = AppSettingsHelper.GetConfig("DocumentDBTable");

        private CloudStorageAccount storageAccount;

        private CloudTable table;

        public CosmosDBService()
        {
            try
            {
                storageAccount = CloudStorageAccount.Parse(DocumentDBConnectionString);
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
                table = tableClient.GetTableReference(CosmosTableName);

                if (table.CreateIfNotExistsAsync().Result)
                    Console.WriteLine("Created Table named: {0}", CosmosTableName);
                else
                    Console.WriteLine("Table {0} already exists", CosmosTableName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Please confirm the AccountName and AccountKey CosmosDB", ex?.Message);
            }
        }

        public object GetItem(string partitionKey, Guid documentId)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Console.WriteLine("GetItem CosmosDB", ex?.Message);
            }

            return null;
        }

        public object PutItem(object document)
        {
            try
            {
                if (document != null && document is TableEntity)
                {
                    TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(document as TableEntity);
                    TableResult result = table.ExecuteAsync(insertOrMergeOperation).Result;
                    return result.Result as TableEntity;
                }
                else
                    Console.WriteLine("PutItem CosmosDB - Document is null Or Document is not type TableEntity");
            }
            catch (Exception ex)
            {
                Console.WriteLine("PutItem CosmosDB", ex?.Message);
            }

            return null;
        }
    }
}
