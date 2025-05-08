using System.Collections;
using System.Runtime.CompilerServices;

namespace Grouping;

/// <summary>
///     
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TElement"></typeparam>
public class GroupedList<TKey, TElement> : IList<TElement>, IGrouping<TKey, TElement>, ICollection<TElement>, IEnumerable<TElement>, IEnumerable
{
    internal TElement[] _items;
    internal int _size;
    internal int _version;

    /// <inheritdoc />
    public TKey Key { get; }

    /// <inheritdoc />
    public TElement this[int index]
    {
        get => _items[index];
        set => _items[index] = value;
    }

    /// <inheritdoc />
    public int Count
        => _size;

    public GroupedList(IGrouping<TKey, TElement> enumerable)
    {
        _items = [.. enumerable];
        _size = _items.Length;

        Key = enumerable.Key;
    }

    public GroupedList(TKey key, IEnumerable<TElement> enumerable)
    {
        _items = [.. enumerable];
        _size = _items.Length;

        Key = key;
    }

    public void Add(TElement item)
    {
        _size++;
        Array.Resize(ref _items, _size);

        _items[^1] = item;

        _version++;
    }

    public void Clear()
    {
        // It's already 0, don't do anything.
        if (_size == 0)
            return;

        Array.Clear(_items);

        _size = 0;
        _version++;
    }

    public bool Contains(TElement item)
        => _items.Length != 0 && IndexOf(item) >= 0;

    public void CopyTo(TElement[] array, int arrayIndex)
        => Array.Copy(_items, 0, array!, arrayIndex, _size);

    public IEnumerator<TElement> GetEnumerator()
        => new Enumerator(this);

    public int IndexOf(TElement item)
        => Array.IndexOf(_items, item);

    public void Insert(int index, TElement item)
    {
        if ((uint)index > (uint)_size)
            throw new IndexOutOfRangeException("The index must be more than zero and equal to or less than the size -plus one during insertion- of the collection.");

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

    public bool Remove(TElement item)
    {
        var indexOf = IndexOf(item);

        if (indexOf >= 0)
        {
            RemoveAt(indexOf);
            return true;
        }

        return false;
    }

    public void RemoveAt(int index)
    {
        if ((uint)index >= (uint)_size)
            throw new IndexOutOfRangeException("The index must be more than zero and equal to or less than the size of the collection.");

        _size--;
        if (index < _size)
            Array.Copy(_items, index + 1, _items, index, _size - index);

        if (RuntimeHelpers.IsReferenceOrContainsReferences<TElement>())
            _items[_size] = default!;

        _version++;
    }

    bool ICollection<TElement>.IsReadOnly
        => false;

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public struct Enumerator : IEnumerator<TElement>, IEnumerator
    {
        private readonly GroupedList<TKey, TElement> _list;
        private int _index;
        private readonly int _version;
        private TElement? _current;

        internal Enumerator(GroupedList<TKey, TElement> list)
        {
            _list = list;
            _index = 0;
            _version = list._version;
            _current = default;
        }

        public readonly void Dispose()
        {
        }

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

        private bool MoveNextRare()
        {
            if (_version != _list._version)
                throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");

            _index = _list._size + 1;
            _current = default;
            return false;
        }

        public readonly TElement Current => _current!;

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
    }
}
