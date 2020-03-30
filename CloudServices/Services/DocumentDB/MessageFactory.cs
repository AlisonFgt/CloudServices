using CloudServices.Common;
using System;

namespace CloudServices.Services.DocumentDB
{
    public static class MessageFactory
    {
        public static IMessageDB Create(string partitionKey, string instance, string documentId, string payload)
        {
            string cloud = AppSettingsHelper.GetCloud();

            switch (cloud)
            {
                case "azure":
                    return new MessageCosmosDB(partitionKey, instance, documentId, payload);

                default:
                    return new MessageDynamoDB(partitionKey, instance, documentId, payload);
            }
        }
    }
}
