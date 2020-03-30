using System;

namespace CloudServices.Services.DocumentDB
{
    public interface IDocumentDBService
    {
        dynamic PutItem(object document);

        dynamic GetItem(string partitionKey, string rowKey);
    }
}
