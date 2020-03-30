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

        public IMessageDB GetItem(string partitionKey, string rowKey)
        {
            try
            {
                TableOperation retrieveOperation = TableOperation.Retrieve<MessageCosmosDB>(partitionKey, rowKey);
                TableResult result = table.ExecuteAsync(retrieveOperation).Result;
                dynamic customer = result.Result as dynamic;
                if (customer != null)
                {
                    Console.WriteLine("\t{0}\t{1}\t{2}\t{3}", customer.PartitionKey, customer.RowKey, customer.CreatedAt, customer.Payload);
                    return customer;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetItem CosmosDB", ex?.Message);
            }

            return null;
        }

        public dynamic PutItem(IMessageDB document)
        {
            try
            {
                if (document != null && document is TableEntity)
                {
                    TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(document as TableEntity);
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
            throw new NotImplementedException();
        }
    }
}
