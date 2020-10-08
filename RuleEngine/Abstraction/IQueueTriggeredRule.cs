using System;
using System.Collections.Generic;
using System.Text;

namespace RuleEngine.Abstraction
{
    public interface IQueueTriggeredRule
    {
        public string TopicName { get; set; }
        public int Partition { get; set; }
        public void ActivateListener();
    }
}
