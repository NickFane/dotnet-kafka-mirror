using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using DotnetKafkaMirror.Configuration;
using Serilog;

namespace DotnetKafkaMirror.Kafka
{
    public class KafkaConsumer : IKafkaConsumer
    {
        private readonly Consumer<string, string> _consumer;

        public KafkaConsumer(IEnvironmentConfigProvider configProvider)
        {
            var config = configProvider.GetEnvironmentConfig();
            _consumer = new Consumer<string, string>(config.SourceKafkaConfig, new StringDeserializer(Encoding.UTF8),
                new StringDeserializer(Encoding.UTF8));

            _consumer.Subscribe(config.SourceTopics);
            _consumer.OnPartitionsAssigned += ConsumerOnOnPartitionsAssigned;
            _consumer.OnPartitionsRevoked += ConsumerOnOnPartitionsRevoked;

            _consumer.OnError += OnError;
        }

        public Message<string, string> GetMessage()
        {
            _consumer.Consume(out var message, TimeSpan.FromSeconds(1));
            return message;
        }

        private void OnError(object sender, Error e)
        {
            var exception = new Exception(e.Reason);
            Log.Error(exception, "{@e}", e);
        }

        private void ConsumerOnOnPartitionsRevoked(object sender, List<TopicPartition> topicPartitions)
        {
            Log.Information($"Revoked partitions: [{string.Join(", ", topicPartitions)}]");
            _consumer.Unassign();
        }

        private void ConsumerOnOnPartitionsAssigned(object o, List<TopicPartition> topicPartitions)
        {
            Log.Information($"Assigned partitions: [{string.Join(", ", topicPartitions)}], member id: {_consumer.MemberId}");
            _consumer.Assign(topicPartitions);
        }
    }

    public interface IKafkaConsumer
    {
        Message<string, string> GetMessage();
    }
}
