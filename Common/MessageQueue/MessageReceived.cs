namespace Common.MessageQueue
{
    public class MessageReceived
    {
        public MessageReceived(string topic, string message,
            long currentOffset, long highOffset, long lastCommittedOffset)
        {
            Message = message;
            Topic = topic;
            LastCommittedOffset = lastCommittedOffset;
            CurrentOffset = currentOffset;
            HighOffset = highOffset;
        }
        public string Message { get; set; }
        public string Topic { get; set; }
        public long LastCommittedOffset { get; set; }
        public long CurrentOffset { get; set; }
        public long HighOffset { get; set; }
    }
}
