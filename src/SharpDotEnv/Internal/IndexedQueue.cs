// Copyright 2023-2024 Ville Penttinen
// Distributed under the MIT License.
// https://github.com/vipentti/SharpDotEnv/blob/main/LICENSE.md

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SharpDotEnv.Internal;

internal class IndexedQueue<T> : IReadOnlyList<T>
{
    private T[] _array;
    private int _start;
#pragma warning disable IDE0032 // Use auto property
    private int _len;
#pragma warning restore IDE0032 // Use auto property

    public IndexedQueue()
        : this(8) { }

    public IndexedQueue(int capacity)
    {
        _array = new T[capacity];
        _len = 0;
        _start = 0;
    }

    public void Enqueue(T item)
    {
        if (_len == _array.Length)
        {
            var newArray = new T[_array.Length * 2];
            for (var i = 0; i < _array.Length; ++i)
            {
                newArray[i] = _array[(_start + i) % _len];
            }
            _start = 0;
            _array = newArray;
        }

        _array[(_start + _len) % _array.Length] = item;
        ++_len;
    }

    public bool TryPeek(out T item)
    {
        if (_len > 0)
        {
            item = Peek();
            return true;
        }

        item = default!;
        return false;
    }

    public T Peek()
    {
        ThrowIfEmpty();

        return _array[_start];
    }

    public bool TryDequeue(out T item)
    {
        if (_len > 0)
        {
            item = Dequeue();
            return true;
        }

        item = default!;
        return false;
    }

    public T Dequeue()
    {
        ThrowIfEmpty();
        var result = _array[_start];
        _start = (_start + 1) % _array.Length;
        --_len;
        return result;
    }

    private void ThrowIfEmpty()
    {
        if (_len == 0)
        {
            throw new InvalidOperationException("IndexedQueue is empty.");
        }
    }

    public void Clear()
    {
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            int len = _len;
            _start = 0;
            _len = 0;
            if (len > 0)
            {
                Array.Clear(_array, 0, len); // Clear elements so gc can reclaim the references.
            }
        }
        else
        {
            _start = 0;
            _len = 0;
        }
#else
        // When RuntimeHelpers is not supported
        // we pessimistically clear the array
        int len = _len;
        _start = 0;
        _len = 0;
        if (len > 0)
        {
            Array.Clear(_array, 0, len); // Clear elements so gc can reclaim possible references
        }
#endif
    }

    public int LastIndexOf(T value) =>
        LastIndexOf(it => EqualityComparer<T>.Default.Equals(value, it));

    public int LastIndexOf(Func<T, bool> pred)
    {
        for (int i = _len - 1; i >= 0; --i)
        {
            if (pred(this[i]))
            {
                return i;
            }
        }

        return -1;
    }

    public Enumerator GetEnumerator() => new Enumerator(this);

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => new Enumerator(this);

    IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

    public int Count => _len;

    public T this[int index] => _array[(_start + index) % _array.Length];

    public struct Enumerator : IEnumerator<T>
    {
        private readonly IndexedQueue<T> _queue;
        private int _index;

        public Enumerator(IndexedQueue<T> queue)
            : this()
        {
            _index = -1;
            _queue = queue;
            Current = default!;
        }

        public void Reset()
        {
            _index = -1;
        }

        public T Current { get; private set; }

        readonly object IEnumerator.Current => Current!;

        public bool MoveNext()
        {
            var next = _index + 1;
            var que = _queue;

            if (next < que.Count)
            {
                Current = que[next];
                _index = next;
                return true;
            }

            return false;
        }

        public readonly void Dispose() { }
    }
}
