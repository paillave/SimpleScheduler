namespace Paillave.Scheduler.Core;

public interface IJobDefinitionService<TJobDefinition, TKey> where TKey : IEquatable<TKey>
{
    void SetJobDefinition(TJobDefinition source);
    void UnSetJobDefinition(TKey key);
    void ResyncJobDefinitions();
    void Trigger(TKey key);
}
