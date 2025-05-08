using System.Collections;

namespace Grouping;

/// <summary>
///     A <see cref="IGrouping{TKey, TElement}"/> collection that is grouped by a key.
/// </summary>
/// <typeparam name="TKey">The key type of this grouping.</typeparam>
/// <typeparam name="T">The element type of this grouping.</typeparam>
public class Grouping<TKey, T> : IGrouping<TKey, T>
    where TKey : notnull
{
    private readonly IEnumerable<T> _elements;

    /// <inheritdoc />
    public TKey Key { get; }

    /// <summary>
    ///     Creates a new instance of <see cref="Grouping{TKey, TElement}"/> with the specified grouping.
    /// </summary>
    /// <param name="enumerable"></param>
    public Grouping(IGrouping<TKey, T> enumerable)
    {
        _elements = enumerable;

        Key = enumerable.Key;
    }

    /// <summary>
    ///     Creates a new instance of <see cref="Grouping{TKey, TElement}"/> with the specified key and enumerable.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="enumerable"></param>
    public Grouping(TKey key, IEnumerable<T> enumerable)
    {
        _elements = enumerable;

        Key = key;
    }

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator() 
        => _elements.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() 
        => GetEnumerator();
}