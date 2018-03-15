using System;
using System.Collections.Generic;
using System.Text;

namespace FakeMirrorMaker.Models
{
    public class MirrorConfig
    {
        public string SourceBrokerList { get; set; }
        public string DestinationBrokerList { get; set; }
        public string SourceTopics { get; set; }
        public string ConsumerGroup { get; set; }
        public string DestinationTopic { get; set; }

        public Dictionary<string, object> SourceKafkaConfig => new Dictionary<string, object>()
        {
            {"group.id", ConsumerGroup},
            {"bootstrap.servers", SourceBrokerList},
            {"auto.offset.reset", "beginning"}
        };

        public Dictionary<string, object> DestinationKafkaConfig => new Dictionary<string, object>()
        {
            {"bootstrap.servers", DestinationBrokerList},
        };

    }
}
