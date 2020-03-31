namespace CloudServices.Services.DocumentDB
{
    public interface IDocumentDBService
    {
        dynamic PutItem(IMessageDB document);

        IMessageDB GetItem(string partitionKey, string rowKey);

        IMessageDB DeleteItem(string partitionKey, string rowKey);
    }
}
