using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using CloudServices.Common;
using System;

namespace CloudServices.Services.Queue
{
    public class AmazonSimpleQueue : IQueueService
    {
        string _regionName = AppSettingsHelper.GetConfig("AWSRegionName");

        string _queueName = AppSettingsHelper.GetConfig("QueueName");

        RegionEndpoint _endPoint;

        public AmazonSimpleQueue()
        {
            try
            {
                _endPoint = RegionEndpoint.GetBySystemName(_regionName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("AmazonSimpleQueue" + ex?.Message);
            }
        }

        public bool SendMessage(string message)
        {
            try
            {
                using (var client = GetClient())
                {
                    client.SendMessage(new SendMessageRequest(_queueName, message));
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("AmazonSimpleQueue - SendMessage - " + ex?.Message);
            }

            return false;
        }

        internal IAmazonSQS GetClient() => new AmazonSQSClient(_endPoint);
    }
}
