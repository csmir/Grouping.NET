using System.Collections;

namespace Grouping;

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