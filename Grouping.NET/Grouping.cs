using System.Collections;

namespace Grouping;

/// <summary>
///     A <see cref="IGrouping{TKey, TElement}"/> collection that is grouped by a key.
/// </summary>
/// <typeparam name="TKey">The key type of this grouping.</typeparam>
/// <typeparam name="T">The element type of this grouping.</typeparam>
public class Grouping<TKey, TElement> : IGrouping<TKey, TElement>, IEnumerable<TElement>, IEnumerable
    where TKey : notnull
{
    private readonly IEnumerable<TElement> _elements;

    /// <inheritdoc />
    public TKey Key { get; }

    public Grouping(IGrouping<TKey, TElement> enumerable)
    {
        _elements = enumerable;

        Key = enumerable.Key;
    }

    public Grouping(TKey key, IEnumerable<TElement> enumerable)
    {
        _elements = enumerable;

        Key = key;
    }

    /// <inheritdoc />
    public IEnumerator<TElement> GetEnumerator() 
        => _elements.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() 
        => GetEnumerator();
}