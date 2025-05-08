using System.Collections;

namespace Grouping;

/// <summary>
///     A <see cref="IList{T}"/> collection that is grouped by a key.
/// </summary>
/// <typeparam name="TKey">The key type of this grouped list.</typeparam>
/// <typeparam name="T">The element type of this grouped list.</typeparam>
public class GroupedList<TKey, T> : List<T>, IGrouping<TKey, T>, ICollection<T>, IEnumerable<T>, IEnumerable
    where TKey : notnull
{
    /// <inheritdoc />
    public TKey Key { get; }

    /// <summary>
    ///     Creates a new instance of <see cref="GroupedList{TKey, TElement}"/> with the specified key.
    /// </summary>
    /// <param name="key">The object to consider the key of this grouped list.</param>
    public GroupedList(TKey key, int capacity)
        : base(capacity)
    {
        Key = key;
    }

    /// <summary>
    ///     Creates a new instance of <see cref="GroupedList{TKey, TElement}"/> with the specified grouping.
    /// </summary>
    /// <param name="enumerable">The grouping to consider the key and elements of this grouped list.</param>
    public GroupedList(IGrouping<TKey, T> enumerable)
        : base(enumerable)
    {
        Key = enumerable.Key;
    }

    /// <summary>
    ///     Creates a new instance of <see cref="GroupedList{TKey, TElement}"/> with the specified key and enumerable.
    /// </summary>
    /// <param name="key">The object to consider the key of this grouped list.</param>
    /// <param name="enumerable">The values to consider the elements of this grouped list.</param>
    public GroupedList(TKey key, IEnumerable<T> enumerable)
        : base(enumerable)
    {
        Key = key;
    }
}
