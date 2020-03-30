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
                table.CreateIfNotExistsAsync().Wait();                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Please confirm the AccountName and AccountKey CosmosDB", ex?.Message);
            }
        }

        public IMessageDB GetItem(string partitionKey, string rowKey)
        {
            try
            {
                TableOperation retrieveOperation = TableOperation.Retrieve<MessageCosmosDB>(partitionKey, rowKey);
                TableResult result = table.ExecuteAsync(retrieveOperation).Result;
                MessageCosmosDB item = result.Result as MessageCosmosDB;
                return item;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetItem CosmosDB", ex?.Message);
                return null;
            }            
        }

        public dynamic PutItem(IMessageDB document)
        {
            try
            {
                if (document != null && document is MessageCosmosDB)
                {
                    TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(document as MessageCosmosDB);
                    TableResult result = table.ExecuteAsync(insertOrMergeOperation).Result;
                    return result.Result as IMessageDB;
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

        public IMessageDB DeleteItem(string partitionKey, string rowKey)
        {
            try
            {
                TableOperation retrieveOperation = TableOperation.Retrieve<MessageCosmosDB>(partitionKey, rowKey);
                TableResult result = table.ExecuteAsync(retrieveOperation).Result;
                var deleteEntity = (MessageCosmosDB)result.Result;
                TableOperation delete = TableOperation.Delete(deleteEntity);
                table.ExecuteAsync(delete).Wait();
                return deleteEntity;
            }
            catch (Exception ex)
            {
                Console.WriteLine("DeleteItem CosmosDB", ex?.Message);
                return null;
            }
        }
    }
}
