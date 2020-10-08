using Common.MessageQueue;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Abstraction
{
    public interface IMessageQueueReceiver
    {
        event EventHandler<MessageReceivedEventArgs> OnMessageReceived;
        IEnumerable<(string, int)> TopicPartitions { get; set; }
        string GroupId { get; set; }
        void ReceiveInNewThread();
        void ReceiveNonBlocking();
        MessageReceived ReadLastMessage();
    }
}
