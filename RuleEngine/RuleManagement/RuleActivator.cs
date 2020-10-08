using Common.Logging;
using RuleEngine.Abstraction;
using System;

namespace RuleEngine.RuleManagement
{
    public class RuleActivator : IRuleActivator
    {
        private readonly IRuleConfigurator ruleConfigurator;

        public RuleActivator(IRuleConfigurator ruleConfigurator)
        {
            this.ruleConfigurator = ruleConfigurator;
        }

        public void ActivateRules()
        {
            var activeQueueRules = ruleConfigurator.ConfigureQueueTriggeredRules();

            foreach (var rule in activeQueueRules)
            {
                try
                {
                    rule.ActivateListener();

                }
                catch (Exception ex)
                {
                    ConsoleTrace.Log(ex.Message + ex.StackTrace);
                    continue;
                }
            }
        }
    }
}
