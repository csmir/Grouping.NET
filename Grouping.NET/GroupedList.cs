using System.Collections;
using System.Runtime.CompilerServices;

namespace Grouping;

/// <summary>
///     A <see cref="IList{T}"/> collection that is grouped by a key.
/// </summary>
/// <typeparam name="TKey">The key type of this grouped list.</typeparam>
/// <typeparam name="T">The element type of this grouped list.</typeparam>
public class GroupedList<TKey, T> : IList<T>, IGrouping<TKey, T>, ICollection<T>, IEnumerable<T>, IEnumerable
    where TKey : notnull
{
    private readonly int _originalCapacity = 0;

    internal T[] _items;
    internal int _size;
    internal int _version;

    /// <inheritdoc />
    public TKey Key { get; }

    /// <inheritdoc />
    public T this[int index]
    {
        get => _items[index];
        set => _items[index] = value;
    }

    /// <inheritdoc />
    public int Count
        => _size;

    /// <summary>
    ///     Creates a new instance of <see cref="GroupedList{TKey, TElement}"/> with the specified key.
    /// </summary>
    /// <param name="key">The object to consider the key of this grouped list.</param>
    public GroupedList(TKey key, int capacity)
    {
        if (capacity < 0)
            throw new ArgumentOutOfRangeException(nameof(capacity), "The capacity must be more than or equal to zero.");

        _originalCapacity = capacity;

        _items = new T[_originalCapacity];
        _size = 0;
        _version = 0;

        Key = key;
    }

    /// <summary>
    ///     Creates a new instance of <see cref="GroupedList{TKey, TElement}"/> with the specified grouping.
    /// </summary>
    /// <param name="enumerable">The grouping to consider the key and elements of this grouped list.</param>
    public GroupedList(IGrouping<TKey, T> enumerable)
    {
        _items = [.. enumerable];
        _size = _items.Length;
        _version = 0;

        Key = enumerable.Key;
    }

    /// <summary>
    ///     Creates a new instance of <see cref="GroupedList{TKey, TElement}"/> with the specified key and enumerable.
    /// </summary>
    /// <param name="key">The object to consider the key of this grouped list.</param>
    /// <param name="enumerable">The values to consider the elements of this grouped list.</param>
    public GroupedList(TKey key, IEnumerable<T> enumerable)
    {
        _items = [.. enumerable];
        _size = _items.Length;
        _version = 0;

        Key = key;
    }

    /// <inheritdoc />
    public void Add(T item)
    {
        if (_size == _items.Length)
        {
            // Resize the array to make room for the new item.
            Array.Resize(ref _items, _size + 1);
        }
        else if (_size < _items.Length)
        {
            // Shift the items to the right to make room for the new item.
            Array.Copy(_items, _size, _items, _size + 1, _items.Length - _size - 1);
        }

        // Add the new item to the end of the array.
        _items[_size] = item;
        _size++;
        _version++;
    }

    /// <inheritdoc />
    public void Clear()
    {
        // It's already 0, don't do anything.
        if (_size == 0)
            return;

        Array.Clear(_items);

        _size = 0;
        _version++;
    }

    /// <inheritdoc />
    public bool Contains(T item)
        => _items.Length != 0 && IndexOf(item) >= 0;

    /// <inheritdoc />
    public void CopyTo(T[] array, int arrayIndex)
        => Array.Copy(_items, 0, array!, arrayIndex, _size);

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator()
        => new Enumerator(this);

    /// <inheritdoc />
    public int IndexOf(T item)
        => Array.IndexOf(_items, item);

    /// <inheritdoc />
    public void Insert(int index, T item)
    {
        if ((uint)index > (uint)_size)
            throw new ArgumentOutOfRangeException(nameof(index), "The index must be more than zero and equal to or less than the size -plus one during insertion- of the collection.");

        if (_size == _items.Length)
        {
            Array.Resize(ref _items, _size + 1);
            Array.Copy(_items, index, _items, index + 1, _items.Length - index - 1);
        }
        else if (index < _size)
            Array.Copy(_items, index, _items, index + 1, _size - index);

        _items[index] = item;
        _size++;
        _version++;
    }

    /// <inheritdoc />
    public bool Remove(T item)
    {
        var indexOf = IndexOf(item);

        if (indexOf >= 0)
        {
            RemoveAt(indexOf);
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public void RemoveAt(int index)
    {
        if ((uint)index >= (uint)_size)
            throw new ArgumentOutOfRangeException(nameof(index), "The index must be more than zero and equal to or less than the size of the collection.");

        _size--;
        if (index < _size)
            Array.Copy(_items, index + 1, _items, index, _size - index);

        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            _items[_size] = default!;

        _version++;
    }

    public struct Enumerator : IEnumerator<T>, IEnumerator
    {
        private readonly GroupedList<TKey, T> _list;
        private int _index;
        private readonly int _version;
        private T? _current;

        internal Enumerator(GroupedList<TKey, T> list)
        {
            _list = list;
            _index = 0;
            _version = list._version;
            _current = default;
        }

        /// <inheritdoc />
        public readonly void Dispose()
        {
        }

        /// <inheritdoc />
        public bool MoveNext()
        {
            var localList = _list;

            if (_version == localList._version && ((uint)_index < (uint)localList._size))
            {
                _current = localList._items[_index];
                _index++;
                return true;
            }
            return MoveNextRare();
        }

        /// <inheritdoc />
        private bool MoveNextRare()
        {
            if (_version != _list._version)
                throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");

            _index = _list._size + 1;
            _current = default;
            return false;
        }

        /// <inheritdoc />
        public readonly T Current => _current!;

        #region Internal

        readonly object? IEnumerator.Current
        {
            get
            {
                if (_index == 0 || _index == _list._size + 1)
                    throw new InvalidOperationException("Enumeration has either not started or has already finished.");

                return Current;
            }
        }

        void IEnumerator.Reset()
        {
            if (_version != _list._version)
                throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");

            _index = 0;
            _current = default;
        }

        #endregion
    }

    #region Internal

    bool ICollection<T>.IsReadOnly
        => false;

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    #endregion

}
