using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using CloudServices.Common;
using CloudServices.Common.Extesions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;

namespace CloudServices.Services.Storage
{
    public class S3StorageService : IStorageService
    {
        private static IAmazonS3 DefaultAmazonClient()
        {
            var appRegionName = AppSettingsHelper.GetConfig("AWSRegionName");
            var endPoint = RegionEndpoint.GetBySystemName(appRegionName);

            return new AmazonS3Client(endPoint);
        }

        public void UploadByPath(string bucketNameAndDirPath, string objectName, string filePath)
        {
            var bucketNameAndPath = GetBucketNameAndPath(bucketNameAndDirPath);
            var bucketName = bucketNameAndPath.Key;
            var directoryPath = bucketNameAndPath.Value;

            if (!BucketExists(bucketName))
            {
                throw new Exception(string.Format("The bucket {0} does not exist.", bucketName));
            }

            var putRequest = new PutObjectRequest
            {
                FilePath = filePath,
                BucketName = bucketName,
                CannedACL = S3CannedACL.PublicRead,
                Key = Key(directoryPath, objectName)
            };

            using (var client = DefaultAmazonClient())
            {
                var response = client.PutObject(putRequest);

                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    throw new Exception(string.Format("Error on upload {0}", objectName));
                }
            }
        }

        public void UploadByImage(string bucketNameAndDirPath, string objectName, Image image)
        {
            var inputStream = image.ToStream(ImageHelper.GetImageFormat(objectName));

            UploadByStream(bucketNameAndDirPath, objectName, inputStream);
        }

        public void UploadByHttpPostedFile(string bucketNameAndDirPath, string objectName, IFormFile file)
        {
            UploadByStream(bucketNameAndDirPath, objectName, file.OpenReadStream());
        }

        public void UploadByStream(string bucketNameAndDirPath, string objectName, Stream fileStream)
        {
            var splittedName = GetBucketNameAndPath(bucketNameAndDirPath);
            var bucketName = splittedName.Key;

            if (!BucketExists(bucketName))
            {
                throw new Exception(string.Format("The bucket {0} does not exist.", bucketName));
            }

            var putRequest = new PutObjectRequest
            {
                InputStream = fileStream,
                BucketName = bucketName,
                CannedACL = S3CannedACL.PublicRead,
                Key = Key(splittedName.Value, objectName)
            };

            using (var client = DefaultAmazonClient())
            {
                var response = client.PutObject(putRequest);

                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    throw new Exception(string.Format("Error on upload {0}", objectName));
                }
            }
        }

        public void Delete(string bucketNameAndDirPath, string objectName)
        {
            var splittedName = GetBucketNameAndPath(bucketNameAndDirPath);
            var bucketName = splittedName.Key;

            if (!BucketExists(bucketName))
            {
                throw new Exception(string.Format("The bucket {0} does not exist.", bucketName));
            }

            var key = Key(splittedName.Value, objectName);

            using (var client = DefaultAmazonClient())
            {
                client.DeleteObject(bucketName, key);
            }
        }

        public string GetUrl(string bucketNameAndDirPath, string objectName)
        {
            return GetUrl(bucketNameAndDirPath, objectName, 365);
        }

        public virtual string GetUrl(string bucketNameAndDirPath, string objectName, int expireDays)
        {
            var bucketNameAndPath = GetBucketNameAndPath(bucketNameAndDirPath);
            var bucketName = bucketNameAndPath.Key;
            var directoryPath = bucketNameAndPath.Value;
            var expiryUrlRequest = new GetPreSignedUrlRequest
            {
                BucketName = bucketName,
                Key = Key(directoryPath, objectName),
                Expires = DateTime.Now.AddDays(expireDays)
            };

            using (var client = DefaultAmazonClient())
            {
                return client.GetPreSignedURL(expiryUrlRequest);
            }
        }

        public Image GetImage(string bucketNameAndDirPath, string objectName)
        {
            var bucketNameAndPath = GetBucketNameAndPath(bucketNameAndDirPath);
            var bucketName = bucketNameAndPath.Key;
            var directoryPath = bucketNameAndPath.Value;

            using (var client = DefaultAmazonClient())
            {
                var objectResponse = client.GetObject(bucketName, Key(directoryPath, objectName));

                return Image.FromStream(objectResponse.ResponseStream);
            }
        }

        public void Download(string bucketNameAndDirPath, string objectName, string fileToDownloadTo)
        {
            var bucketNameAndPath = GetBucketNameAndPath(bucketNameAndDirPath);
            var bucketName = bucketNameAndPath.Key;
            var directoryPath = bucketNameAndPath.Value;

            using (var client = DefaultAmazonClient())
            {
                var objectResponse = client.GetObject(bucketName, Key(directoryPath, objectName));

                using (var fileStream = new FileStream(fileToDownloadTo, FileMode.CreateNew, FileAccess.Write))
                {
                    objectResponse.ResponseStream.CopyTo(fileStream);
                }

                objectResponse.ResponseStream.Close();
            }
        }

        

        internal virtual string Key(string directoryPath, string fileName)
        {
            return Path.Combine(directoryPath, fileName).Replace("\\", "/");
        }

        internal virtual bool BucketExists(string bucketName)
        {
            using (var client = DefaultAmazonClient())
            {
                return AmazonS3Util.DoesS3BucketExist(client, bucketName);
            }
        }

        private KeyValuePair<string, string> GetBucketNameAndPath(string completePath)
        {
            if (!completePath.Contains("/"))
            {
                return new KeyValuePair<string, string>(completePath, null);
            }

            var bucketNameAndPrefix = completePath.Split(new[] { '/' }, 2);

            return new KeyValuePair<string, string>(bucketNameAndPrefix[0], bucketNameAndPrefix[1]);
        }
    }
}
