using Microsoft.AspNetCore.Http;
using System.Drawing;
using System.IO;

namespace CloudServices.Services.Storage
{
    public interface IStorageService
    {
        void UploadByPath(string containerName, string fileNameCloud, string filePathName);

        void UploadByImage(string containerName, string blobName, Image image);

        void UploadByHttpPostedFile(string containerName, string blobName, IFormFile file);

        void UploadByStream(string containerName, string blobName, Stream stream);

        void Delete(string containerName, string blobName);

        string GetUrl(string containerName, string blobName);

        string GetUrl(string containerName, string blobName, int expireDays);

        Image GetImage(string containerName, string blobName);

        void Download(string containerName, string blobName, string fileToDownloadTo);
    }
}
