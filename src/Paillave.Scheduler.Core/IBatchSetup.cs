namespace Paillave.Scheduler.Core;

public interface IBatchSetup<TJobDefinition, TKey> where TKey : IComparable<TKey>, IEquatable<TKey>
{
    IEnumerable<TJobDefinition> GetAll();
    string? GetCronExpression(TJobDefinition source);
    TKey GetKey(TJobDefinition source);
    void Execute(TJobDefinition source, CancellationToken stoppingToken);
}
