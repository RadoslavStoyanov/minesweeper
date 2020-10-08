using Common.Abstraction;
using Common.Logging;
using Common.MessageQueue;
using Microsoft.Extensions.DependencyInjection;
using RuleEngine.Abstraction;
using System;
using System.Collections.Generic;

namespace RuleEngine.Rules
{
    public class GameInitializatorRule : IQueueTriggeredRule
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        //private const string RULE_IN_TOPIC = "GameInitializatorIn";

        public string TopicName { get; set; }
        public int Partition { get; set; }

        public GameInitializatorRule(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public void ActivateListener()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                try
                {
                    var kafkaReceiver = scope.ServiceProvider.GetService<IMessageQueueReceiver>();

                    kafkaReceiver.GroupId = TopicName;
                    kafkaReceiver.TopicPartitions = new List<(string, int)>() { (TopicName, Partition) };
                    kafkaReceiver.OnMessageReceived += ExecutePackager;
                    kafkaReceiver.ReceiveInNewThread();
                }
                catch (Exception ex)
                {
                    ConsoleTrace.LogError(ex.Message);
                }
            }
        }

        public void ExecutePackager(object sender, MessageReceivedEventArgs message)
        {
            // process incoming Kafka message
        }
    }
}
