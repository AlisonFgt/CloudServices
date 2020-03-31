using CloudServices.Common;

namespace CloudServices.Services.Storage
{
    public static class StorageServiceFactory
    {
        public static IStorageService Create()
        {
            string cloud = AppSettingsHelper.GetCloud();

            switch (cloud)
            {
                case "azure":
                    return new AzureBlobStorageService();

                default:
                    return new S3StorageService();
            }
        }
    }
}
