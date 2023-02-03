namespace Paillave.Scheduler.Core;

public interface ITickEmitterConnection<TJobDefinition, TKey> where TKey : IEquatable<TKey>
{
    IEnumerable<TJobDefinition> GetAll();
    string? GetCronExpression(TJobDefinition source);
    TKey GetKey(TJobDefinition source);
    void Execute(TJobDefinition source, CancellationToken stoppingToken);
}
