namespace CloudServices.Services.Queue.Model
{
    public class Message
    {
        public string MessageId { get; set; }

        public string Body { get; set; }

        public string ReceiptId { get; set; }

        public Queue Queue { get; set; }
    }
}
