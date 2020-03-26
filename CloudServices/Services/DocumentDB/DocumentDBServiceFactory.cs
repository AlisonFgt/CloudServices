using CloudServices.Common;

namespace CloudServices.Services.DocumentDB
{
    public class DocumentDBServiceFactory
    {
        public static IDocumentDBService Create()
        {
            string cloud = AppSettingsHelper.GetCloud();

            switch (cloud)
            {
                case "azure":
                    return new CosmosDBService();

                default:
                    return new DynamoDBService();
            }
        }
    }
}
