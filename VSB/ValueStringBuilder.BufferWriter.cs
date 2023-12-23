using System.Buffers;
using System.Diagnostics;

namespace VSB;

public ref partial struct ValueStringBuilder
{
    private class BufferWriter :
        IBufferWriter<char>
    {
        public BufferWriter(
            ValueStringBuilderCore core)
        {
            this._core = core;
        }

        public void Advance(
            int count)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(count, 0);

            this.CheckDisposed();

            this._core.Length += count;
        }

        public Memory<char> GetMemory(
            int sizeHint = 0)
        {
            throw new NotSupportedException();
        }

        public Span<char> GetSpan(
            int sizeHint = 0)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(sizeHint, 0);

            this.CheckDisposed();

            var lengthToGrow = sizeHint - this._core.FreeCapacity;
            if (lengthToGrow > 0)
            {
                this._core.Grow(lengthToGrow);
            }

            var buffer = this._core.GetBuffer(BufferType.Freespace);

            Debug.Assert(buffer.Length >= sizeHint);

            return buffer;
        }

        private void CheckDisposed()
        {
            this._core.CheckDisposed();
        }

        private readonly ValueStringBuilderCore _core;
    }
}
