using Microsoft.Extensions.Hosting;

namespace Paillave.Scheduler.Core;
internal class SchedulerBackgroundService<TJobDefinition, TKey> : BackgroundService where TKey : IEquatable<TKey>
{
    private readonly  TickEmitterHub<TJobDefinition, TKey> _tickEmitterHub;
    private readonly ITickEmitterConnection<TJobDefinition, TKey> _tickEmitterConnection;
    private TickEmitterService<TJobDefinition, TKey>? _tickEmitterService;

    public SchedulerBackgroundService(ITickEmitterConnection<TJobDefinition, TKey> tickEmitterConnection, TickEmitterHub<TJobDefinition, TKey> tickEmitterHub)
        => (_tickEmitterHub, _tickEmitterConnection) = (tickEmitterHub, tickEmitterConnection);
    protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.Run(() =>
    {
        using EventWaitHandle waitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
        _tickEmitterService = TickEmitterService.Create(_tickEmitterHub);
        _tickEmitterService.Tick += (sender, e) => _tickEmitterConnection.Execute(e, stoppingToken);
        _tickEmitterHub.Stopped += (sender, e) => waitHandle.Set();
        stoppingToken.Register(() => _tickEmitterService.Stop());
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
