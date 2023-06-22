#nullable disable warnings

using System.Collections;

namespace SourceGeneratorUtils;

/// <summary>
/// Provides an immutable list implementation which implements sequence equality.
/// </summary>
public sealed class ImmutableEquatableArray<T> : IEquatable<ImmutableEquatableArray<T>>, IReadOnlyList<T>
    where T : IEquatable<T>
{
    /// <summary>
    /// Gets an empty <see cref="ImmutableEquatableArray{T}"/>.
    /// </summary>
    public static ImmutableEquatableArray<T> Empty { get; } = new(Array.Empty<T>());

    private readonly T[] _values;

    /// <inheritdoc/>
    public T this[int index] => _values[index];

    /// <inheritdoc/>
    public int Count => _values.Length;

    /// <summary>
    /// Creates a new <see cref="ImmutableEquatableArray{T}"/> from the given values.
    /// </summary>
    /// <param name="values">The values.</param>
    public ImmutableEquatableArray(IEnumerable<T> values)
        => _values = values as T[] ?? values.ToArray();

    /// <inheritdoc/>
    public bool Equals(ImmutableEquatableArray<T>? other)
        => other != null && ((ReadOnlySpan<T>)_values).SequenceEqual(other._values);

    /// <inheritdoc/>
    public override bool Equals(object? obj)
        => obj is ImmutableEquatableArray<T> other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        int hash = 0;
        foreach (T value in _values)
        {
            hash = HashHelpers.Combine(hash, value is null ? 0 : value.GetHashCode());
        }

        return hash;
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator that iterates through the collection.</returns>
    public Enumerator GetEnumerator() => new(_values);

    /// <inheritdoc/>
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => ((IEnumerable<T>)_values).GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => _values.GetEnumerator();

    /// <summary>
    /// An enumerator for <see cref="ImmutableEquatableArray{T}"/>.
    /// </summary>
    public struct Enumerator
    {
        private readonly T[] _values;
        private int _index;

        internal Enumerator(T[] values)
        {
            _values = values;
            _index = -1;
        }

        /// <summary>
        /// Moves to the next value.
        /// </summary>
        /// <returns>Whether the operation was successful.</returns>
        public bool MoveNext()
        {
            int newIndex = _index + 1;

            if ((uint)newIndex < (uint)_values.Length)
            {
                _index = newIndex;
                return true;
            }

            return false;
        }

        /// <summary>
        /// The current value at the current index. 
        /// </summary>
        public readonly T Current => _values[_index];
    }
}

/// <summary>
/// Provides helper methods to create new <see cref="ImmutableEquatableArray{T}"/>s.
/// </summary>
public static class ImmutableEquatableArray
{
    /// <summary>
    /// Gets an empty <see cref="ImmutableEquatableArray{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the values.</typeparam>
    /// <returns>An empty <see cref="ImmutableEquatableArray{T}"/>.</returns>
    public static ImmutableEquatableArray<T> Empty<T>() where T : IEquatable<T>
        => ImmutableEquatableArray<T>.Empty;

    /// <summary>
    /// Creates a new <see cref="ImmutableEquatableArray{T}"/> from the given values.
    /// </summary>
    /// <typeparam name="T">The type of the values.</typeparam>
    /// <param name="values">The values</param>
    /// <returns>A new <see cref="ImmutableEquatableArray{T}"/> from the given values.</returns>
    public static ImmutableEquatableArray<T> ToImmutableEquatableArray<T>(this IEnumerable<T> values) where T : IEquatable<T>
        => new(values);

    /// <summary>
    /// Creates a new <see cref="ImmutableEquatableArray{T}"/> from the given values.
    /// </summary>
    /// <typeparam name="T">The type of the values.</typeparam>
    /// <param name="values">The values</param>
    /// <returns>A new <see cref="ImmutableEquatableArray{T}"/> from the given values.</returns>
    public static ImmutableEquatableArray<T> Create<T>(params T[] values) where T : IEquatable<T>
        => values is { Length: > 0 } ? new(values) : ImmutableEquatableArray<T>.Empty;
}

file static class HashHelpers
{
    public static int Combine(int h1, int h2) => (h1 << 5 | h1 >>> 27) + h1 ^ h2;
}