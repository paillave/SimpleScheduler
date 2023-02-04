using Microsoft.Extensions.Hosting;

namespace Paillave.Scheduler.Core;
internal class SchedulerBackgroundService<TJobDefinition, TKey> : BackgroundService
    where TKey : IComparable<TKey>, IEquatable<TKey>
{
    private readonly IBatchSetup<TJobDefinition, TKey> _tickEmitterProvider;
    private TickEmitterService<TJobDefinition, TKey>? _tickEmitterService;
    private readonly JobDefinitionService<TJobDefinition, TKey> _jobDefinitionService;

    public SchedulerBackgroundService(IBatchSetup<TJobDefinition, TKey> tickEmitterProvider, JobDefinitionService<TJobDefinition, TKey> jobDefinitionService)
        => (_tickEmitterProvider, _jobDefinitionService) = (tickEmitterProvider, jobDefinitionService);
    protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.Run(() =>
    {
        using EventWaitHandle waitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
        _tickEmitterService = new TickEmitterService<TJobDefinition, TKey>(_tickEmitterProvider, stoppingToken);
        _jobDefinitionService.SetTickEmitterService(_tickEmitterService);
        stoppingToken.Register(() => waitHandle.Set());
        _tickEmitterService.Start();
        waitHandle.WaitOne();
    }, stoppingToken);
    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _tickEmitterService?.Stop();
        return base.StopAsync(cancellationToken);
    }
    public override void Dispose()
    {
        base.Dispose();
        _tickEmitterService?.Dispose();
    }
}

internal class JobDefinitionService<TJobDefinition, TKey> : IJobDefinitionService<TJobDefinition, TKey>
    where TKey : IComparable<TKey>, IEquatable<TKey>
{
    private TickEmitterService<TJobDefinition, TKey>? _tickEmitterService;
    internal void SetTickEmitterService(TickEmitterService<TJobDefinition, TKey> tickEmitterService)
        => _tickEmitterService = tickEmitterService;
    public void SetJobDefinition(TJobDefinition source)
        => _tickEmitterService?.UpdateJobDefinition(source);
    public void UnSetJobDefinition(TKey key)
        => _tickEmitterService?.RemoveJobDefinition(key);
    public void ResyncJobDefinitions()
        => _tickEmitterService?.RefreshJobDefinitions();
    public void Trigger(TKey key)
        => _tickEmitterService?.Trigger(key);
}
