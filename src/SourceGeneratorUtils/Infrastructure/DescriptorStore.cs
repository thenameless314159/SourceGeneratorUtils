using System.Collections;

namespace SourceGeneratorUtils;

internal sealed class DescriptorStore<TDescriptor> : IReadOnlyDictionary<string, ITypeSpec>
    where TDescriptor : ITypeSpec
{
    public readonly IReadOnlyDictionary<string, TDescriptor> _descriptors;

    public ITypeSpec this[string key] => _descriptors[key];

    public IEnumerable<string> Keys => _descriptors.Keys;

    public IEnumerable<ITypeSpec> Values
    {
        get
        {
            foreach (var kvp in _descriptors)
                yield return kvp.Value;
        }
    }

    public int Count => _descriptors.Count;

    public DescriptorStore(IReadOnlyDictionary<string, TDescriptor>? descriptors = null)
        => _descriptors = descriptors ?? new Dictionary<string, TDescriptor>(0);

    public bool ContainsKey(string key) => _descriptors.ContainsKey(key);

    public bool TryGetValue(string key, out ITypeSpec value)
    {
        if (_descriptors.TryGetValue(key, out var descriptor))
        {
            value = descriptor;
            return true;
        }

        value = default!;
        return false;
    }

    public IEnumerator<KeyValuePair<string, ITypeSpec>> GetEnumerator() => new Enumerator(_descriptors.GetEnumerator());

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private sealed class Enumerator : IEnumerator<KeyValuePair<string, ITypeSpec>>
    {
        private readonly IEnumerator<KeyValuePair<string, TDescriptor>> _enumerator;

        public Enumerator(IEnumerator<KeyValuePair<string, TDescriptor>> enumerator)
            => _enumerator = enumerator;

        public bool MoveNext() => _enumerator.MoveNext();
        public void Reset() => _enumerator.Reset();

        KeyValuePair<string, ITypeSpec> IEnumerator<KeyValuePair<string, ITypeSpec>>.Current 
            => new(_enumerator.Current.Key, _enumerator.Current.Value);

        public object Current => _enumerator.Current;

        public void Dispose() => _enumerator.Dispose();
    }
}