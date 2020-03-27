using System;

namespace CloudServices.Services.DocumentDB
{
    public interface IDocumentDBService
    {
        object PutItem(object document);

        object GetItem(string partitionKey, Guid documentId);
    }
}
