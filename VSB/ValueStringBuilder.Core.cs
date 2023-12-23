using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace VSB;

public ref partial struct ValueStringBuilder
{
    private sealed unsafe class ValueStringBuilderCore :
        IDisposable
    {
        public ValueStringBuilderCore(
            Span<char> initialBuffer)
        {
            this._buffer = GetPointer(initialBuffer);
            this._capacity = initialBuffer.Length;
        }

        public void Append(
            ReadOnlySpan<char> value)
        {
            if (value.IsEmpty)
            {
                return;
            }

            var valueLength = value.Length;
            var lengthToGrow = valueLength - this.FreeCapacity;

            if (lengthToGrow > 0)
            {
                this.Grow(lengthToGrow);
            }

            value.CopyTo(this.GetBuffer(BufferType.Freespace));
            this._position += valueLength;
        }

        public override string ToString()
        {
            if (this._position == 0)
            {
                return string.Empty;
            }

            return new string(this.GetBuffer(BufferType.Content));
        }

        public void Dispose()
        {
            if (this._disposed)
            {
                return;
            }

            this._owner?.Dispose();
            GC.SuppressFinalize(this);

            this._disposed = true;

            ValueStringBuilderEventSource.Log.Disposed();
        }

        public int Length
        {
            get
            {
                return this._position;
            }

            set
            {
                Debug.Assert(value >= 0);

                var lengthToGrow = value - this._capacity;
                if (lengthToGrow > 0)
                {
                    this.Grow(lengthToGrow);
                }

                this._position = value;
            }
        }

        internal void Grow(
            int lengthToGrow)
        {
            Debug.Assert(lengthToGrow > 0);

            var capacity = this._capacity;
            lengthToGrow = Math.Max(lengthToGrow, capacity);

            var newCapacity = capacity + lengthToGrow;
            var newMemory = MemoryPool<char>.Shared.Rent(newCapacity);
            var newBuffer = newMemory.Memory.Span;

            this.GetBuffer(BufferType.Content).CopyTo(newBuffer);
            newBuffer.Slice(this._position).Fill('\0');

            this._owner?.Dispose();

            this._owner = newMemory;
            this._capacity = newCapacity;
            this._buffer = GetPointer(newBuffer);

            ValueStringBuilderEventSource.Log.Grown(lengthToGrow);
        }

        internal Span<char> GetBuffer(
            BufferType type)
        {
            var buffer = new Span<char>(this._buffer, this._capacity);

            switch (type)
            {
                case BufferType.Entire:
                    return buffer;

                case BufferType.Content:
                    return buffer.Slice(0, this._position);

                case BufferType.Freespace:
                    return buffer.Slice(this._position);
            }

            throw new ArgumentOutOfRangeException(nameof(type));
        }

        internal int FreeCapacity
        {
            get
            {
                return this._capacity - this._position;
            }
        }

        internal void CheckDisposed()
        {
            ObjectDisposedException.ThrowIf(this._disposed, typeof(ValueStringBuilder));
        }

        private static char* GetPointer(
            Span<char> span)
        {
            return (char*)Unsafe.AsPointer(ref span[0]);
        }

        private IMemoryOwner<char>? _owner = null;

        private char* _buffer;

        private int _capacity;

        private int _position = 0;

        private bool _disposed = false;
    }

    internal enum BufferType
    {
        Entire,
        Content,
        Freespace
    }
}
