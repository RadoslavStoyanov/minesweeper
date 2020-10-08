using System;

namespace Common.MessageQueue
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public MessageReceivedEventArgs(string topic, string message, long currentOffset)
        {
            Message = message;
            Topic = topic;
            CurrentOffset = currentOffset;
        }
        public MessageReceivedEventArgs(string topic, string message)
        {
            Message = message;
            Topic = topic;
            CurrentOffset = 0;
        }
        public string Message { get; set; }
        public string Topic { get; set; }
        public long CurrentOffset { get; set; }
    }
}