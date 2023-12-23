using System.Buffers;

namespace VSB;

public ref partial struct ValueStringBuilder
{
    public ValueStringBuilder(
        Span<char> initialBuffer)
    {
        this._core = new ValueStringBuilderCore(initialBuffer);
    }

    public void Append(
        string? value)
    {
        this.CheckDisposed();

        if (string.IsNullOrEmpty(value))
        {
            return;
        }

        this.AppendNoCheck(value.AsSpan());
    }

    public void Append(
        scoped ReadOnlySpan<char> value)
    {
        this.CheckDisposed();

        if (value.IsEmpty)
        {
            return;
        }

        this.AppendNoCheck(value);
    }

    public void Append(
        char value)
    {
        this.CheckDisposed();

        this.AppendNoCheck([value]);
    }

    public void Clear()
    {
        this.Length = 0;
    }

    public IBufferWriter<char> GetWriter()
    {
        this.CheckDisposed();

        return new BufferWriter(this._core);
    }

    // using 変数のプロパティは設定できないので、Length をセットする代わりに使う
    public void SetLength(
        int length)
    {
        this.Length = length;
    }

    public char[] ToArray()
    {
        return this._core.GetBuffer(BufferType.Content).ToArray();
    }

    public override string ToString()
    {
        this.CheckDisposed();

        return this._core.ToString();
    }

    public bool TryCopyTo(
        scoped Span<char> destination)
    {
        this.CheckDisposed();

        return this.TryCopyToNoCheck(destination, 0, this.Length);
    }

    public bool TryCopyTo(
        scoped Span<char> destination,
        int offset,
        int length)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(offset, 0);
        ArgumentOutOfRangeException.ThrowIfLessThan(length, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(offset, this.Length);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(offset + length, this.Length);

        this.CheckDisposed();

        return this.TryCopyToNoCheck(destination, offset, length);
    }

    public bool TryCopyTo(
        scoped Span<char> destination,
        Range range)
    {
        this.CheckDisposed();

        var (offset, length) = range.GetOffsetAndLength(this.Length);

        return this.TryCopyToNoCheck(destination, offset, length);
    }

    public void Dispose()
    {
        this._core.Dispose();
    }

    public int Length
    {
        get
        {
            this.CheckDisposed();

            return this._core.Length;
        }

        set
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(value, 0);

            this.CheckDisposed();

            this._core.Length = value;
        }
    }

    private void AppendNoCheck(
        scoped ReadOnlySpan<char> value)
    {
        this._core.Append(value);
    }

    private bool TryCopyToNoCheck(
        scoped Span<char> destination,
        int offset,
        int length)
    {
        var source = this._core.GetBuffer(BufferType.Content).Slice(offset, length);
        return source.TryCopyTo(destination);
    }

    private void CheckDisposed()
    {
        this._core.CheckDisposed();
    }

    private readonly ValueStringBuilderCore _core;
}
