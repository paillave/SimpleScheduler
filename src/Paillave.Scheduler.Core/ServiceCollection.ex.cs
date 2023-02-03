// using Microsoft.Extensions.DependencyInjection;

// namespace Paillave.Scheduler.Core;

// public static class ServiceCollectionEx
// {
//     public static IServiceCollection AddScheduler<TJobDefinition, TKey>(this IServiceCollection serviceCollection, ITickEmitterConnection<TJobDefinition, TKey> tickEmitterConnection) where TKey : IEquatable<TKey>
//     {
//         var tickEmitterHub = new TickEmitterHub<TJobDefinition, TKey>(tickEmitterConnection);
//         serviceCollection.AddSingleton(tickEmitterConnection);
//         serviceCollection.AddSingleton(tickEmitterHub);
//         serviceCollection.AddSingleton((IJobDefinitionService<TJobDefinition, TKey>)tickEmitterHub);
//         serviceCollection.AddHostedService<SchedulerBackgroundService<TJobDefinition, TKey>>();
//         return serviceCollection;
//     }
// }




using Microsoft.Extensions.DependencyInjection;

namespace Paillave.Scheduler.Core;

public static class ServiceCollectionEx
{
    public static IServiceCollection AddScheduler<TJobDefinition, TKey>(this IServiceCollection serviceCollection, ITickEmitterConnection<TJobDefinition, TKey> tickEmitterConnection) where TKey : IEquatable<TKey>
    {
        var tickEmitterHub = new TickEmitterHub<TJobDefinition, TKey>(tickEmitterConnection);
        serviceCollection.AddSingleton(tickEmitterConnection);
        serviceCollection.AddSingleton(tickEmitterHub);
        serviceCollection.AddSingleton((IJobDefinitionService<TJobDefinition, TKey>)tickEmitterHub);
        serviceCollection.AddHostedService<SchedulerBackgroundService<TJobDefinition, TKey>>();
        return serviceCollection;
    }
    public static IServiceCollection AddScheduler<TJobDefinition, TKey>(this IServiceCollection serviceCollection) where TKey : IEquatable<TKey>
    {
        serviceCollection.AddSingleton(i =>
        {
            ITickEmitterConnection<TJobDefinition, TKey> tickEmitterConnection = i.GetService<ITickEmitterConnection<TJobDefinition, TKey>>();
            var tickEmitterHub = new TickEmitterHub<TJobDefinition, TKey>(tickEmitterConnection);
            // return (IJobDefinitionService<TJobDefinition, TKey>)tickEmitterHub;
            return tickEmitterHub;
        });
        serviceCollection.AddSingleton(i=>(IJobDefinitionService<TJobDefinition, TKey>)i.GetService<TickEmitterHub<TJobDefinition, TKey>>());
        serviceCollection.AddHostedService<SchedulerBackgroundService<TJobDefinition, TKey>>();
        return serviceCollection;
    }
}