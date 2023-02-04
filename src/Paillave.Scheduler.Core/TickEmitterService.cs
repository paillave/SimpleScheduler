namespace Paillave.Scheduler.Core;
internal class TickEmitterService<TJobDefinition, TKey> : IDisposable where TKey : IComparable<TKey>, IEquatable<TKey>
{
    private class SourceOccurrence : IDisposable
    {
        public SourceOccurrence(TickEmitter<TJobDefinition> tickSource, IDisposable releaser) => (TickSource, Releaser) = (tickSource, releaser);
        public TickEmitter<TJobDefinition> TickSource { get; }
        public IDisposable Releaser { get; }
        public void Dispose()
        {
            this.Releaser.Dispose();
            this.TickSource.Dispose();
        }
    }
    public void Start()
    {
        foreach (var item in _tickSources)
            item.Value.TickSource.Start();
    }
    public void Stop()
    {
        foreach (var item in _tickSources)
            item.Value.TickSource.Stop();
    }
    private readonly object _lock = new object();
    private readonly IBatchSetup<TJobDefinition, TKey> _tickEmitterProvider;
    private readonly CancellationToken _stoppingToken;
    protected virtual void OnPushTick(TJobDefinition source)
        => Task.Run(() => this._tickEmitterProvider.Execute(source, _stoppingToken));
    public TickEmitterService(IBatchSetup<TJobDefinition, TKey> tickEmitterProvider, CancellationToken stoppingToken)
    {
        _stoppingToken = stoppingToken;
        _stoppingToken.Register(this.Stop);
        _tickEmitterProvider = tickEmitterProvider;
        this.ResetJobDefinitions(this._tickEmitterProvider.GetAll());
    }
    private readonly Dictionary<TKey, SourceOccurrence> _tickSources = new Dictionary<TKey, SourceOccurrence>();
    private bool disposedValue;

    public void UpdateJobDefinition(TJobDefinition source)
    {
        lock (_lock)
        {
            if (_tickSources.TryGetValue(_tickEmitterProvider.GetKey(source), out var occurrence)) occurrence.TickSource.UpdateSource(source);
            else AddJobDefinition(source);
        }
    }
    public void RemoveJobDefinition(TKey sourceKey)
    {
        lock (_lock)
        {
            if (!_tickSources.TryGetValue(sourceKey, out var occurrence)) return;
            if (occurrence == null) return;
            RemoveJobDefinition(occurrence);
        }
    }
    public void RefreshJobDefinitions()
    {
        lock (_lock)
        {
            this.ResetJobDefinitions(this._tickEmitterProvider.GetAll());
        }
    }
    public void Trigger(TKey key)
    {
        if (_tickSources.TryGetValue(key, out var jobDefinition))
            OnPushTick(jobDefinition.TickSource.JobDefinition);
    }
    private void AddJobDefinition(TJobDefinition source)
    {
        lock (_lock)
        {
            var tickSource = new TickEmitter<TJobDefinition>(source, _tickEmitterProvider.GetCronExpression);
            var releaser = tickSource.Subscribe(OnPushTick);
            var occurrence = new SourceOccurrence(tickSource, releaser);
            _tickSources[_tickEmitterProvider.GetKey(source)] = occurrence;
            tickSource.Start();
        }
    }
    private void RemoveJobDefinition(SourceOccurrence occurrence)
    {
        lock (_lock)
        {
            occurrence.Dispose();
            _tickSources.Remove(_tickEmitterProvider.GetKey(occurrence.TickSource.JobDefinition));
        }
    }
    private void ResetJobDefinitions(IEnumerable<TJobDefinition> newSources)
    {
        lock (_lock)
        {
            var updates = _tickSources.Join(newSources, i => i.Key, _tickEmitterProvider.GetKey, (l, r) => new { Previous = l.Value.TickSource, New = r });
            foreach (var tickSource in updates)
                tickSource.Previous.UpdateSource(tickSource.New);
            var toAdd = newSources.Where(n => !_tickSources.ContainsKey(_tickEmitterProvider.GetKey(n))).ToList();
            foreach (var item in toAdd)
                AddJobDefinition(item);
            var toRemove = _tickSources.Values.Where(i => !newSources.Any(n => _tickEmitterProvider.GetKey(n).Equals(_tickEmitterProvider.GetKey(i.TickSource.JobDefinition)))).ToList();
            foreach (var item in toRemove)
                RemoveJobDefinition(item);
        }
    }
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }
            lock (_lock)
            {
                foreach (var item in _tickSources)
                    item.Value.Dispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
