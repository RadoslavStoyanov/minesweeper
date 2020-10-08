using System;
using System.Collections.Generic;
using System.Text;

namespace RuleEngine.Configuration
{
    public class QueueTriggeredRuleConfig
    {
        public string Name { get; set; }

        public string TopicName { get; set; }

        public int Partition { get; set; }

    }
}
