using System;

namespace CloudServices.Services.DocumentDB
{
    public interface IMessageDB
    {
        public string Instance { get; set; }

        public string Guid { get; set; }

        public string PartitionKey { get; set; }

        public string Payload { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
