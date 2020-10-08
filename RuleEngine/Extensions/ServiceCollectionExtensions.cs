using Common.Abstraction;
using Common.MessageQueue;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RuleEngine.Abstraction;
using RuleEngine.Configuration;
using RuleEngine.RuleManagement;
using RuleEngine.Rules;
using System;
using System.Collections.Generic;
using System.Text;

namespace RuleEngine.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection InjectDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();

            var qRuleConfigSection = configuration.GetSection(nameof(QueueTriggeredRuleConfig));
            services.Configure<List<QueueTriggeredRuleConfig>>(qRuleConfigSection);
           
            services.AddTransient<IMessageQueueSender, KafkaSender>();
            services.AddTransient<IMessageQueueReceiver, KafkaReceiver>();

            services.AddSingleton<IRuleActivator, RuleActivator>();
            services.AddSingleton<IRuleConfigurator, RuleConfigurator>();

            services.AddTransient<IQueueTriggeredRule, GameInitializatorRule>();

            //services.AddHostedService<Worker>();

            return services;
        }
    }
}
