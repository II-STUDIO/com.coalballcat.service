using System;
using System.Collections.Generic;

namespace Coalballcat.Services
{
    public class IndexBaseQueue<T>
    {
        private T[] _buffer;
        private int _head; // points to dequeue position
        private int _tail; // points to enqueue position
        private int _count;

        public int Count => _count;

        public IndexBaseQueue(int capacity)
        {
            if (capacity <= 0) throw new ArgumentException("Capacity must be > 0");
            _buffer = new T[capacity];
            _head = 0;
            _tail = 0;
            _count = 0;
        }

        /// <summary>
        /// Adds item at the end of the queue.
        /// Throws if capacity exceeded.
        /// </summary>
        public void Enqueue(T item)
        {
            if (_count == _buffer.Length)
                throw new InvalidOperationException("Queue capacity exceeded");

            _buffer[_tail] = item;
            _tail = (_tail + 1) % _buffer.Length;
            _count++;
        }

        /// <summary>
        /// Removes and returns the item at the front of the queue.
        /// </summary>
        public T Dequeue()
        {
            if (_count == 0)
                throw new InvalidOperationException("Queue is empty");

            T item = _buffer[_head];
            _buffer[_head] = default!; // clear reference for GC
            _head = (_head + 1) % _buffer.Length;
            _count--;
            return item;
        }

        /// <summary>
        /// Removes and returns the item at the given index (0 = front of queue).
        /// </summary>
        public T DequeueAt(int index)
        {
            if (index < 0 || index >= _count)
                throw new ArgumentOutOfRangeException(nameof(index));

            int realIndex = (_head + index) % _buffer.Length;
            T item = _buffer[realIndex];

            // Shift elements to fill the gap
            for (int i = realIndex; i != _tail; i = (i + 1) % _buffer.Length)
            {
                int next = (i + 1) % _buffer.Length;
                if (next != _tail)
                    _buffer[i] = _buffer[next];
                else
                    _buffer[i] = default!;
            }

            _tail = (_tail == 0) ? _buffer.Length - 1 : _tail - 1;
            _count--;

            return item;
        }


        /// <summary>
        /// Returns item at front without removing.
        /// </summary>
        public T Peek()
        {
            if (_count == 0)
                throw new InvalidOperationException("Queue is empty");

            return _buffer[_head];
        }

        /// <summary>
        /// Attempts to remove first occurrence of item from queue.
        /// Returns true if found and removed.
        /// </summary>
        public bool TryRemove(T item)
        {
            if (_count == 0) return false;

            int index = -1;
            EqualityComparer<T> comparer = EqualityComparer<T>.Default;

            // Find item in circular buffer
            for (int i = 0; i < _count; i++)
            {
                int bufferIndex = (_head + i) % _buffer.Length;
                if (comparer.Equals(_buffer[bufferIndex], item))
                {
                    index = bufferIndex;
                    break;
                }
            }

            if (index == -1) return false;

            // Shift elements to fill gap, maintaining circular order
            for (int i = index; i != _tail; i = (i + 1) % _buffer.Length)
            {
                int next = (i + 1) % _buffer.Length;
                if (next != _tail)
                    _buffer[i] = _buffer[next];
                else
                    _buffer[i] = default!;
            }

            _tail = (_tail == 0) ? _buffer.Length - 1 : _tail - 1;
            _count--;
            return true;
        }

        /// <summary>
        /// Clears the queue.
        /// </summary>
        public void Clear()
        {
            if (_count == 0) return;

            if (_head < _tail)
            {
                Array.Clear(_buffer, _head, _count);
            }
            else
            {
                Array.Clear(_buffer, _head, _buffer.Length - _head);
                Array.Clear(_buffer, 0, _tail);
            }

            _head = 0;
            _tail = 0;
            _count = 0;
        }

        /// <summary>
        /// Gets the element at specified queue index (0 = front).
        /// </summary>
        public T GetAt(int index)
        {
            if (index < 0 || index >= _count)
                throw new ArgumentOutOfRangeException(nameof(index));
            return _buffer[(_head + index) % _buffer.Length];
        }
    }
}