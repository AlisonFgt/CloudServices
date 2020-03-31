using System;

namespace CloudServices.Services.DocumentDB
{
    public static class DocumentDBTest
    {
        public static void Start()
        {
            Console.WriteLine("Start Queue Tests Azure - Service Bus || AWS - SQS");
            var documentoDBService = DocumentDBServiceFactory.Create();
            var partitionKey = "12345";
            var instance = "HLG-HEINZ";
            var payload = "{\"ID\":1,\"Name\":\"Alison\",\"Address\":\"Canoas\"}";
            var guid = Guid.NewGuid().ToString();
            IMessageDB doc = MessageFactory.Create(partitionKey, instance, guid, payload);
            documentoDBService.PutItem(doc);
            Console.WriteLine("Send Item!");
            var message = documentoDBService.GetItem(partitionKey, guid);
            Console.WriteLine(message.Payload);
            Console.WriteLine("Get Item");
            var deleted = documentoDBService.DeleteItem(partitionKey, guid);
            Console.WriteLine(deleted.Payload);
            Console.WriteLine("Delete Item");
        }
    }
}
