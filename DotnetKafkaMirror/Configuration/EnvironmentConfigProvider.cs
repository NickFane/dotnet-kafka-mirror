using System;
using FakeMirrorMaker.Models;

namespace DotnetKafkaMirror.Configuration
{
    public class EnvironmentConfigProvider : IEnvironmentConfigProvider
    {
        private const string SourceBrokerListKey = "source_broker_list";
        private const string DestinationBrokerListKey = "destination_broker_list";
        private const string TopicsKey = "topics";
        private const string ConsumerGroupKey = "consumer_group";
        private const string DestinationTopicKey = "destination_topic";

        private MirrorConfig _cachedConfig;

        public MirrorConfig GetEnvironmentConfig(bool useCache = true)
        {
            if (_cachedConfig != null && useCache)
            {
                return _cachedConfig;
            }

            _cachedConfig = new MirrorConfig
            {
                SourceBrokerList = GetEnvironmentVariable(SourceBrokerListKey),
                DestinationBrokerList = GetEnvironmentVariable(DestinationBrokerListKey),
                SourceTopics = GetEnvironmentVariable(TopicsKey),
                ConsumerGroup = GetEnvironmentVariable(ConsumerGroupKey),
                DestinationTopic = GetEnvironmentVariable(DestinationTopicKey, false)
            };

            return _cachedConfig;
        }

        private string GetEnvironmentVariable(string key, bool required = true)
        {
            var variable = Environment.GetEnvironmentVariable(key);
            if (required && string.IsNullOrEmpty(variable))
            {
                throw new ArgumentNullException($"Environment variable {key} is missing");
            }
            return variable;
        }

    }

    public interface IEnvironmentConfigProvider
    {
        MirrorConfig GetEnvironmentConfig(bool useCache = true);
    }
}
