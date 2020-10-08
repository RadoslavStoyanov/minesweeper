using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Common.Abstraction;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace Common.MessageQueue
{
    public class KafkaReceiver : IMessageQueueReceiver
    {
        public event EventHandler<MessageReceivedEventArgs> OnMessageReceived;
        private readonly string bootstrapServers;

        public IEnumerable<(string, int)> TopicPartitions { get; set; }
        public string GroupId { get; set; }

        public KafkaReceiver(IConfiguration configuration)
        {
            bootstrapServers = configuration.GetSection("KafkaConfiguration")["Host"];
        }

        public void ReceiveInNewThread()
        {
            var thread = new Thread(Receive);
            thread.Start();
        }

        public MessageReceived ReadLastMessage()
        {
            var consumerConfig = new ConsumerConfig()
            {
                BootstrapServers = bootstrapServers,
                GroupId = GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
            };
            if (TopicPartitions.Count() != 1)
                throw new Exception("Method does not support multiple topic partitions");

            using (var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build())
            {
                var kafkaTopicPartition = TopicPartitions
                    .Where(tp => !string.IsNullOrEmpty(tp.Item1))
                    .Select(x => new TopicPartition(x.Item1, x.Item2)).SingleOrDefault();
                var committedOffset = consumer.Committed(new List<TopicPartition>() { kafkaTopicPartition },
                    TimeSpan.FromSeconds(5)).Single().Offset;

                var offsets = consumer.QueryWatermarkOffsets(kafkaTopicPartition, TimeSpan.FromSeconds(5));
                var highOffset = offsets?.High ?? 0;
                if (highOffset == 0)
                    return null;

                consumer.Assign(new TopicPartitionOffset(kafkaTopicPartition, highOffset - 1));

                var consumeResult = consumer.Consume(CancellationToken.None);
                consumer.Commit(consumeResult);

                return new MessageReceived(consumeResult.Topic,
                    consumeResult.Message.Value,
                    consumeResult.Offset.Value, highOffset, committedOffset);
            }
        }

        public void ReceiveNonBlocking()
        {
            var consumerConfig = new ConsumerConfig()
            {
                BootstrapServers = bootstrapServers,
                GroupId = GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
            };

            using (var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build())
            {
                var kafkaTopicPartitions = TopicPartitions
                    .Where(tp => !string.IsNullOrEmpty(tp.Item1))
                    .Select(x => new TopicPartition(x.Item1, x.Item2));

                var committedOffsets = consumer.Committed(kafkaTopicPartitions, TimeSpan.FromSeconds(5));

                var readableTopicPartitions = kafkaTopicPartitions.Select(x =>
                {
                    var offsets = consumer.QueryWatermarkOffsets(x, TimeSpan.FromSeconds(5));
                    var highOffset = offsets?.High ?? 0;
                    var commitedOffset = committedOffsets.First(c => c.Topic == x.Topic && c.Partition == x.Partition).Offset;
                    if (commitedOffset == highOffset || highOffset == 0)
                        return null;
                    return new { TopicPartition = x, x.Topic, x.Partition, HighOffset = highOffset };
                }).ToList();
                readableTopicPartitions.RemoveAll(x => x == null);
                consumer.Assign(readableTopicPartitions.Select(x => x.TopicPartition));

                try
                {
                    while (readableTopicPartitions.Count() > 0)
                    {
                        try
                        {
                            if (OnMessageReceived != null)
                            {
                                var consumeResult = consumer.Consume(CancellationToken.None);
                                consumer.Commit(consumeResult);

                                OnMessageReceived.Invoke(this,
                                    new MessageReceivedEventArgs(consumeResult.Topic, consumeResult.Value,
                                    consumeResult.Offset.Value));


                                var currentTopicPartition = readableTopicPartitions.First(x => x.Topic == consumeResult.Topic
                                && x.Partition == consumeResult.Partition);
                                if (consumeResult.Offset.Value == currentTopicPartition.HighOffset - 1)
                                {
                                    readableTopicPartitions.Remove(currentTopicPartition);
                                    consumer.Assign(readableTopicPartitions.Select(x => x.TopicPartition));
                                }

                            }
                            else
                            {
                                Thread.Sleep(5000);
                            }
                        }
                        catch (ConsumeException e)
                        {
                            //logger.Error($"Consume error: {e.Error.Reason}");
                        }
                    }
                }
                catch (Exception e)
                {
                    //logger.Error($"Consume error: {e.Message}");
                }
                finally
                {
                    consumer.Close();
                    consumer.Dispose();
                }
            }
        }

        private void Receive()
        {
            var consumerConfig = new ConsumerConfig()
            {
                BootstrapServers = bootstrapServers,
                GroupId = GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            using (var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build())
            {
                var kafkaTopicPartitions = TopicPartitions.Select(x => new TopicPartition(x.Item1, x.Item2));
                consumer.Assign(kafkaTopicPartitions);
                try
                {
                    while (true)
                    {
                        try
                        {
                            if (OnMessageReceived != null)
                            {
                                var consumeResult = consumer.Consume(CancellationToken.None);

                                consumer.Commit(consumeResult);

                                OnMessageReceived.Invoke(this,
                                    new MessageReceivedEventArgs(consumeResult.Topic, consumeResult.Value,
                                    consumeResult.Offset.Value));
                            }
                            else
                            {
                                Thread.Sleep(5000);
                            }
                        }
                        catch (ConsumeException e)
                        {
                            //logger.Error($"Consume error: {e.Error.Reason}");
                        }
                    }
                }
                catch (Exception e)
                {
                    //logger.Error($"Consume error: {e.Message}");
                }
                finally
                {
                    consumer.Close();
                    consumer.Dispose();
                }
            }
        }
    }
}
