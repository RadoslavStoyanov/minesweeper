using System.Collections.Generic;

namespace RuleEngine.Abstraction
{
    public interface IRuleConfigurator
    {
        IEnumerable<IQueueTriggeredRule> ConfigureQueueTriggeredRules();
    }
}