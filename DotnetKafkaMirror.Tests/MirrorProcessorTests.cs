using System;
using System.Collections.Generic;
using System.Text;
using DotnetKafkaMirror.Configuration;
using DotnetKafkaMirror.Kafka;
using Moq;
using NUnit.Framework;

namespace DotnetKafkaMirror.Tests
{
    [TestFixture]
    public class MirrorProcessorTests
    {
        private Mock<IKafkaConsumer> _kafkaConsumer;
        private Mock<IKafkaProducer> _kafkaProducer;
        private Mock<IMirrorTopicHandler> _mirrorTopicHandler;
        private Mock<IEnvironmentConfigProvider> _environmentConfigProvider;
        private MirrorProcessor _target;

        [SetUp]
        public void SetUp()
        {
            _kafkaConsumer = new Mock<IKafkaConsumer>();
            _kafkaProducer = new Mock<IKafkaProducer>();
            _mirrorTopicHandler = new Mock<IMirrorTopicHandler>();
            _environmentConfigProvider = new Mock<IEnvironmentConfigProvider>();

            _target = new MirrorProcessor(_kafkaConsumer.Object, _kafkaProducer.Object, _mirrorTopicHandler.Object, _environmentConfigProvider.Object);
        }

        [Test]
        public void runonce_nullmessage_doesnotproduce()
        {
            _kafkaConsumer.Setup(s => s.GetMessage()).Returns(() => null);

            _target.RunOnce();

            _kafkaProducer.Verify(v => v.ProduceAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

    }
}
