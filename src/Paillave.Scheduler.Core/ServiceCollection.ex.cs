using Microsoft.Extensions.DependencyInjection;

namespace Paillave.Scheduler.Core;

public static class ServiceCollectionEx
{
    public static IServiceCollection AddScheduler<TJobDefinition, TKey>(this IServiceCollection serviceCollection, IBatchSetup<TJobDefinition, TKey> tickEmitterProvider) 
        where TKey : IComparable<TKey>, IEquatable<TKey>
    {
        serviceCollection.AddSingleton((IBatchSetup<TJobDefinition, TKey>)tickEmitterProvider);
        serviceCollection.AddSingleton<JobDefinitionService<TJobDefinition, TKey>>();
        serviceCollection.AddSingleton<IJobDefinitionService<TJobDefinition, TKey>>(i => i.GetService<JobDefinitionService<TJobDefinition, TKey>>());
        serviceCollection.AddHostedService<SchedulerBackgroundService<TJobDefinition, TKey>>();
        return serviceCollection;
    }
    public static IServiceCollection AddScheduler<TJobDefinition, TKey, TTickEmitterProvider>(this IServiceCollection serviceCollection)
        where TKey : IComparable<TKey>, IEquatable<TKey>
        where TTickEmitterProvider : class, IBatchSetup<TJobDefinition, TKey>
    {
        serviceCollection.AddSingleton<IBatchSetup<TJobDefinition, TKey>, TTickEmitterProvider>();
        serviceCollection.AddSingleton<JobDefinitionService<TJobDefinition, TKey>>();
        serviceCollection.AddSingleton<IJobDefinitionService<TJobDefinition, TKey>>(i => i.GetService<JobDefinitionService<TJobDefinition, TKey>>());
        serviceCollection.AddHostedService<SchedulerBackgroundService<TJobDefinition, TKey>>();
        return serviceCollection;
    }
}