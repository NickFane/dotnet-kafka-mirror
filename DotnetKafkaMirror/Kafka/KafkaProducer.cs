using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using DotnetKafkaMirror.Configuration;
using Serilog;

namespace DotnetKafkaMirror.Kafka
{
    public class KafkaProducer : IKafkaProducer
    {
        private readonly Producer<string, string> _producer;

        public KafkaProducer(IEnvironmentConfigProvider environmentConfigProvider)
        {
            var config = environmentConfigProvider.GetEnvironmentConfig();

            _producer = new Producer<string, string>(config.DestinationKafkaConfig,
                new StringSerializer(Encoding.UTF8), new StringSerializer(Encoding.UTF8));

            _producer.OnError += OnError;

        }
        public Task ProduceAsync(string topic, string key, string value)
        {
            var produceTask = _producer.ProduceAsync(topic, key, value);
            produceTask.ContinueWith(task =>
            {
                var result = task.Result;
                if (result.Error.HasError)
                {
                    Log.Error(new Exception(result.Error.Reason), "{@error}", result.Error);
                }
            });

            return produceTask;
        }

        private void OnError(object sender, Error e)
        {
            var exception = new Exception(e.Reason);
            Log.Error(exception, "{@e}", e);
        }
    }

    public interface IKafkaProducer
    {
        Task ProduceAsync(string topic, string key, string value);
    }
}
