namespace Paillave.Scheduler.Core;

internal class TickEmitterHub<TJobDefinition, TKey> : IJobDefinitionService<TJobDefinition, TKey> where TKey : IEquatable<TKey>
{
    private readonly ITickEmitterConnection<TJobDefinition, TKey> _tickEmitterConnection;

    public TickEmitterHub(ITickEmitterConnection<TJobDefinition, TKey> tickEmitterConnection)
        => (_tickEmitterConnection) = (tickEmitterConnection);

    public event EventHandler<ITJobDefinitionChange<TJobDefinition, TKey>>? Changed = null;
    protected virtual void OnChanged(ITJobDefinitionChange<TJobDefinition, TKey> e)
        => this.Changed?.Invoke(this, e);
    public event EventHandler? Stopped = null;
    protected virtual void OnStopped()
        => this.Stopped?.Invoke(this, new EventArgs());

    public void Dispose()
        => this.OnStopped();
    public void SetJobDefinition(TJobDefinition source)
        => OnChanged(new SaveJobDefinitionChange<TJobDefinition, TKey>(source));
    public void UnSetJobDefinition(TKey key)
        => OnChanged(new RemoveJobDefinitionChange<TJobDefinition, TKey>(key));
    public IEnumerable<TJobDefinition> GetAll() => this._tickEmitterConnection.GetAll();
    public string? GetCronExpression(TJobDefinition source) => this._tickEmitterConnection.GetCronExpression(source);
    public TKey GetKey(TJobDefinition source) => this._tickEmitterConnection.GetKey(source);
}
