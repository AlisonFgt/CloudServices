using CloudServices.Common;
using CloudServices.Common.Extesions;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Drawing;
using System.IO;

namespace CloudServices.Services.Storage
{
    public class AzureBlobStorageService : IStorageService
    {
        private readonly string _azure_storage_connection_string;

        private string _container;

        public AzureBlobStorageService()
        {
            _azure_storage_connection_string = AppSettingsHelper.GetConfig("StorageConnectionString");
            _container = AppSettingsHelper.GetConfig("StorageContainer");
            SetPublicContainerPermissions();
        }

        public CloudBlobClient DefaultAzureClient()
        {
            CloudStorageAccount storageAccount;
            if (CloudStorageAccount.TryParse(_azure_storage_connection_string, out storageAccount))
                return storageAccount.CreateCloudBlobClient();
            else
                throw new Exception("Azure Storage ConnectionString Invalid");
        }

        public CloudBlockBlob GetBlobInContainer(string containerName, string blobName)
        {
            containerName = Path.Combine(_container, containerName);
            var cloudBlobClient = DefaultAzureClient();
            var containerReference = cloudBlobClient.GetContainerReference(containerName);

            return containerReference.GetBlockBlobReference(blobName);
        }

        public CloudAppendBlob GetAppendBlobInContainer(string containerName, string blobName)
        {
            containerName = Path.Combine(_container, containerName);
            var cloudBlobClient = DefaultAzureClient();
            var containerReference = cloudBlobClient.GetContainerReference(containerName);

            return containerReference.GetAppendBlobReference(blobName);
        }

        public void Delete(string containerName, string blobName)
        {
            var cloudBlockBlob = GetBlobInContainer(containerName, blobName);
            cloudBlockBlob.DeleteIfExistsAsync().Wait();
        }

        public void Download(string containerName, string blobName, string fileToDownloadTo)
        {
            var cloudBlockBlob = GetBlobInContainer(containerName, blobName);
            cloudBlockBlob.DownloadToFileAsync(fileToDownloadTo, FileMode.Create).Wait();
        }

        public Image GetImage(string containerName, string blobName)
        {
            var cloudBlockBlob = GetBlobInContainer(containerName, blobName);
            cloudBlockBlob.Properties.ContentType = "image/" + ImageHelper.GetImageFormat(blobName).ToString().Trim().ToLower();
            cloudBlockBlob.SetPropertiesAsync().Wait();

            using (var memoryStream = new MemoryStream())
            {
                cloudBlockBlob.DownloadToStreamAsync(memoryStream).Wait();
                memoryStream.Seek(0, SeekOrigin.Begin);
                return Image.FromStream(memoryStream);
            }
        }

        public string GetUrl(string containerName, string blobName)
        {
            var cloudBlockBlob = GetBlobInContainer(containerName, blobName);
            return cloudBlockBlob.Uri.AbsoluteUri;
        }

        public string GetUrl(string containerName, string blobName, int expireDays)
        {
            return GetUrl(containerName, blobName);

            //var cloudBlockBlob = GetBlobInContainer(containerName, blobName);
            //var sharedAccess = new SharedAccessBlobPolicy
            //{
            //    SharedAccessExpiryTime = DateTime.UtcNow.AddDays(expireDays),
            //    Permissions = SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Create
            //};

            //var sasToken = cloudBlockBlob.Container.GetSharedAccessSignature(sharedAccess);

            //return cloudBlockBlob.Uri.AbsoluteUri + sasToken;
        }

        public void UploadByHttpPostedFile(string containerName, string blobName, IFormFile file)
        {
            var cloudBlockBlob = GetBlobInContainer(containerName, blobName);
            cloudBlockBlob.UploadFromStreamAsync(file.OpenReadStream()).Wait();
        }

        public void UploadByImage(string containerName, string blobName, Image image)
        {
            var cloudBlockBlob = GetBlobInContainer(containerName, blobName);
            cloudBlockBlob.UploadFromStreamAsync(image.ToStream(ImageHelper.GetImageFormat(blobName))).Wait();
        }

        public void UploadByPath(string containerName, string fileNameCloud, string filePathName)
        {
            var cloudBlockBlob = GetBlobInContainer(containerName, fileNameCloud);
            cloudBlockBlob.UploadFromFileAsync(filePathName).Wait();
        }

        public void UploadByStream(string containerName, string blobName, Stream stream)
        {
            var cloudBlockBlob = GetBlobInContainer(containerName, blobName);
            cloudBlockBlob.UploadFromStreamAsync(stream);
        }

        public void AppendTextAsync(string containerName, string blobName, string text)
        {
            var appendBlob = GetAppendBlobInContainer(containerName, blobName);

            if (!appendBlob.ExistsAsync().GetAwaiter().GetResult())
                appendBlob.CreateOrReplaceAsync().Wait();

            appendBlob.AppendTextAsync(text).Wait();
        }

        public bool ContainerExists(string containerName)
        {
            var cloudBlobClient = DefaultAzureClient();
            var containerReference = cloudBlobClient.GetContainerReference(containerName);
            try
            {
                containerReference.FetchAttributesAsync().Wait();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void SetPublicContainerPermissions(CloudBlobContainer container)
        {
            BlobContainerPermissions permissions = container.GetPermissionsAsync().Result;
            permissions.PublicAccess = BlobContainerPublicAccessType.Container;
        }

        private void SetPublicContainerPermissions()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_azure_storage_connection_string);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference(_container);
            blobContainer.CreateIfNotExistsAsync().Wait();
            blobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob }).Wait();
        }

        private void CreateContainerIfNotExists()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_azure_storage_connection_string);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference(_container);
            blobContainer.CreateIfNotExistsAsync().Wait();
        }
    }
}
