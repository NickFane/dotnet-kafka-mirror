using System;
using System.Collections.Generic;
using System.Text;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using DotnetKafkaMirror.Configuration;
using DotnetKafkaMirror.Kafka;
using FakeMirrorMaker.Models;
using Serilog;

namespace DotnetKafkaMirror
{
    public class MirrorProcessor : IMirrorProcessor
    {
        private readonly IKafkaConsumer _kafkaConsumer;
        private readonly IKafkaProducer _kafkaProducer;
        private readonly IMirrorTopicHandler _mirrorTopicHandler;

        private readonly MirrorConfig _config;
        private bool _useReplicaTopics;

        private bool _disposing;

        public MirrorProcessor(IKafkaConsumer kafkaConsumer, IKafkaProducer kafkaProducer, IMirrorTopicHandler mirrorTopicHandler, IEnvironmentConfigProvider environmentConfigProvider)
        {
            _kafkaConsumer = kafkaConsumer;
            _kafkaProducer = kafkaProducer;
            _mirrorTopicHandler = mirrorTopicHandler;

            _config = environmentConfigProvider.GetEnvironmentConfig();
        }

        public void Run()
        {
            Log.Information("Running dotnet mirror with current setup | sourceBrokerList {source} | destinationBrokerList {destination} | topics {topics} | consumerGroup {consumerGroup}",
                _config.SourceBrokerList, _config.DestinationBrokerList, _config.SourceTopics, _config.ConsumerGroup);

            _useReplicaTopics = string.IsNullOrEmpty(_config.DestinationTopic);
            if (!_useReplicaTopics)
            {
                Log.Information("Messages will be aggregated to destination topic {topic}", _config.DestinationTopic);
            }

            while (!_disposing)
            {
                var message = _kafkaConsumer.GetMessage();
                if (message != null)
                {
                    MirrorMessage(message);
                }
            }
        }

        public void RunOnce()
        {
            var message = _kafkaConsumer.GetMessage();
            if (message != null)
            {
                MirrorMessage(message);
            }
        }

        private void MirrorMessage(Message<string, string> message)
        {
            var topic = _useReplicaTopics
                ? _mirrorTopicHandler.GetMirrorTopic(message.Topic)
                : _config.DestinationTopic;

            _kafkaProducer.ProduceAsync(topic, message.Key, message.Value);
        }

    }

    public interface IMirrorProcessor
    {
        void Run();
        void RunOnce();
    }
}
