using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Config;
using Rebus.Persistence.InMem;
using Rebus.Routing.TypeBased;

namespace Toolkit.Sagas.Rebus.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddRebusMessageQueue(this IServiceCollection services,
        IConfiguration configuration,
        string queueName,
        Type handlerReference,
        Type contractReference,
        bool isSagaOrchestrator,
        params Type[] subscribeToEvents)
    {
        services.AddRebus(options =>
        {
            options
                .Routing(r => r.TypeBased().MapAssemblyOf(contractReference, queueName))
                .Transport(t => t.UseRabbitMq(configuration.GetConnectionString("MessageBroker"),
                    queueName));

            if (isSagaOrchestrator)
            {
                options.Sagas(s => s.StoreInMemory());
            }

            return options;
        }, onCreated: async bus =>
        {
            foreach (var subscribeToEvent in subscribeToEvents)
            {
                await bus.Subscribe(subscribeToEvent);
            }
        });

        services.AutoRegisterHandlersFromAssembly(handlerReference.Assembly);

        return services;
    }
}