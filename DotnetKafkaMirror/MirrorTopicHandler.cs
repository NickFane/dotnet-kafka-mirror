using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetKafkaMirror
{
    public class MirrorTopicHandler : IMirrorTopicHandler
    {
        private readonly Dictionary<string, string> _mirrorTopics = new Dictionary<string, string>();
        private const string MirrorTopicSuffix = ".mirror";

        public string GetMirrorTopic(string originalTopic)
        {
            if (_mirrorTopics.TryGetValue(originalTopic, out string mirrorTopic))
            {
                return mirrorTopic;
            }

            mirrorTopic = $"{originalTopic}{MirrorTopicSuffix}";
            _mirrorTopics.Add(originalTopic, mirrorTopic);
            return mirrorTopic;
        }
    }

    public interface IMirrorTopicHandler
    {
        string GetMirrorTopic(string originalTopic);
    }
}
