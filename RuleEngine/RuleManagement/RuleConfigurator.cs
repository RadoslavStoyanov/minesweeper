using RuleEngine.Abstraction;
using RuleEngine.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RuleEngine.RuleManagement
{
    public class RuleConfigurator : IRuleConfigurator
    {
        private readonly List<QueueTriggeredRuleConfig> _queueRuleConfigurations;
        private readonly IEnumerable<IQueueTriggeredRule> _registeredQueueRules;

        public RuleConfigurator(
            List<QueueTriggeredRuleConfig> queueRuleConfigurations,
                                IEnumerable<IQueueTriggeredRule> registeredQueueRules)
        {
            _queueRuleConfigurations = queueRuleConfigurations;
            _registeredQueueRules = registeredQueueRules;
        }

        public IEnumerable<IQueueTriggeredRule> ConfigureQueueTriggeredRules()
        {
            var activeRules = new List<IQueueTriggeredRule>();

            foreach (var ruleConfig in _queueRuleConfigurations)
            {
                var rule = _registeredQueueRules.FirstOrDefault(r => r.GetType().Name.Equals(ruleConfig.Name));

                if (rule == null)
                {
                    continue;
                }

                rule.Partition = ruleConfig.Partition;
                rule.TopicName = ruleConfig.TopicName;

                activeRules.Add(rule);
            }

            return activeRules;
        }
    }
}
