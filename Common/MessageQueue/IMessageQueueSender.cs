namespace Common.MessageQueue
{
    public interface IMessageQueueSender
    {
        void Send(string topic, string message, string key = null);
    }
}