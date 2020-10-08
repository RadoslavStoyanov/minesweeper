using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using System;

namespace Common.MessageQueue
{
    public class KafkaSender : IMessageQueueSender
    {
        private string BootstrapServers;

        public KafkaSender(IConfiguration configuration)
        {
            BootstrapServers = configuration.GetSection("KafkaConfiguration")["Host"];
        }

        public async void Send(string topic, string message, string key = null)
        {
            Message<string, string> msg = new Message<string, string>();

            var producerConfig = new ProducerConfig
            {
                BootstrapServers = BootstrapServers
            };

            using (var producer = new ProducerBuilder<string, string>(producerConfig).Build())
            {
                try
                {
                    var deliveryReport = await producer.ProduceAsync(topic,
                        new Message<string, string> { Key = "\"" + key + "\"", Value = message });
                }
                catch (Exception ex)
                {

                    throw;
                }

                producer.Flush(TimeSpan.FromSeconds(10));
            }
        }
    }
}
