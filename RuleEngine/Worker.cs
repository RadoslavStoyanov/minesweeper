using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RuleEngine.Abstraction;

namespace RuleEngine
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IRuleActivator _ruleActivator;

        public Worker(IRuleActivator ruleActivator)
        {
            _ruleActivator = ruleActivator;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            ConsoleTrace.Log("RuleEngine Started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _ruleActivator.ActivateRules();

                }
                catch (Exception ex)
                {
                    ConsoleTrace.LogError(ex.Message);

                    continue;
                }

                ConsoleTrace.Log("Rules Activated");
                Thread.Sleep(Timeout.Infinite);
            }
        }
    }
}
