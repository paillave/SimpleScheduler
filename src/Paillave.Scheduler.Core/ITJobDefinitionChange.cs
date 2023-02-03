namespace Paillave.Scheduler.Core;
internal interface ITJobDefinitionChange<TSource, TKey> where TKey : IEquatable<TKey> { }
internal class RemoveJobDefinitionChange<TSource, TKey> : ITJobDefinitionChange<TSource, TKey> where TKey : IEquatable<TKey>
{
    public RemoveJobDefinitionChange(TKey key) => this.Key = key;
    public TKey Key { get; }
}
internal class SaveJobDefinitionChange<TSource, TKey> : ITJobDefinitionChange<TSource, TKey> where TKey : IEquatable<TKey>
{
    public SaveJobDefinitionChange(TSource source) => this.Source = source;
    public TSource Source { get; }
}
