using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using CloudServices.Common;
using System;
using System.Linq;

namespace CloudServices.Services.Queue
{
    public class AmazonSimpleQueue : IQueueService
    {
        string _regionName = AppSettingsHelper.GetConfig("AWSRegionName");

        string _queueName = AppSettingsHelper.GetConfig("QueueName");

        RegionEndpoint _endPoint;

        internal IAmazonSQS GetClient() => new AmazonSQSClient(_endPoint);

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

        public Model.Queue GetQueueUrl(string name)
        {
            try
            {
                using (var client = GetClient())
                {
                    var responseQueueUrl = client.GetQueueUrl(name);

                    if (responseQueueUrl == null)
                    {
                        Console.WriteLine("AmazonSimpleQueue - GetQueueUrl- responseQueueUrl is null");
                    }

                    return new Model.Queue
                    {
                        Name = name,
                        Url = responseQueueUrl.QueueUrl
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("AmazonSimpleQueue - GetQueueUrl - " + ex?.Message);
                return null;
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

        public Model.Message GetMessage(string queue)
        {
            ReceiveMessageResponse response;
            Model.Queue _Queue;
            try
            {
                _Queue = GetQueueUrl(queue);
                using (var client = GetClient())
                {
                    response = client.ReceiveMessage(new ReceiveMessageRequest
                    {
                        QueueUrl = _Queue.Url,
                        MaxNumberOfMessages = 1
                    });

                    if (response == null || response.Messages == null || response.Messages.Count == 0)
                    {
                        Console.WriteLine("AmazonSimpleQueue - GetMessage - ResponseFromServerIsNullOrInvalid");
                    }

                    var result = response.Messages.FirstOrDefault();

                    return result == null ? null : new Model.Message
                    {
                        Body = result.Body,
                        MessageId = result.MessageId,
                        ReceiptId = result.ReceiptHandle,
                        Queue = _Queue
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("AmazonSimpleQueue - GetMessage - " + ex?.Message);
            }

            return null;
        }

        public bool DeleteMessage(string queue)
        {
            throw new NotImplementedException();
        }
    }
}
