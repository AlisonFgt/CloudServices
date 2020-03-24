using System;

namespace CloudServices.Services.DocumentDB
{
    public interface IDocumentDBService
    {
        void LoadTable();

        object PutItem(object document);

        object GetItem(string partitionKey, Guid documentId);
    }
}
