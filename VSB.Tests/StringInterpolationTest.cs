using Xunit;

namespace VSB.Tests;

public sealed class StringInterpolationTest
{
    [Fact]
    public void ISpanFormattableを実装する型のテスト()
    {
        using var vsb = new ValueStringBuilder(stackalloc char[10]);

        var value = 1234567890;

        vsb.Append($"a{value}b");

        Assert.Equal("a1234567890b", vsb.ToString());
    }

    [Fact]
    public void ISpanFormattableを実装する型の左寄せのテスト()
    {
        using var vsb = new ValueStringBuilder(stackalloc char[10]);

        var value = 12345;

        vsb.Append($"a{value,-10}b");

        Assert.Equal("a12345     b", vsb.ToString());
    }

    [Fact]
    public void ISpanFormattableを実装する型の右寄せのテスト()
    {
        using var vsb = new ValueStringBuilder(stackalloc char[10]);

        var value = 12345;

        vsb.Append($"a{value,10}b");

        Assert.Equal("a     12345b", vsb.ToString());
    }

    [Fact]
    public void すごく長いISpanFormattableを足すテスト()
    {
        using var vsb = new ValueStringBuilder(stackalloc char[10]);

        var value = new SpanFormattable(12345);

        vsb.Append($"a{value}b");

        Assert.Equal("a12345b", vsb.ToString());
    }

    [Fact]
    public void IFormattableを実装する型のテスト()
    {
        using var vsb = new ValueStringBuilder(stackalloc char[10]);

        var value = new Formattable(1234567890);

        vsb.Append($"a{value}b");

        Assert.Equal("a1234567890b", vsb.ToString());
    }

    [Fact]
    public void IFormattableを実装する型の左寄せのテスト()
    {
        using var vsb = new ValueStringBuilder(stackalloc char[10]);

        var value = new Formattable(12345);

        vsb.Append($"a{value,-10}b");

        Assert.Equal("a12345     b", vsb.ToString());
    }

    [Fact]
    public void IFormattableを実装する型の右寄せのテスト()
    {
        using var vsb = new ValueStringBuilder(stackalloc char[10]);

        var value = new Formattable(12345);

        vsb.Append($"a{value,10}b");

        Assert.Equal("a     12345b", vsb.ToString());
    }

    [Fact]
    public void IConvertibleを実装する型のテスト()
    {
        using var vsb = new ValueStringBuilder(stackalloc char[10]);

        var value = new Convertible(1234567890);

        vsb.Append($"a{value}b");

        Assert.Equal("a1234567890b", vsb.ToString());
    }
    

    [Fact]
    public void IConvertibleを実装する型の左寄せのテスト()
    {
        using var vsb = new ValueStringBuilder(stackalloc char[10]);

        var value = new Convertible(12345);

        vsb.Append($"a{value,-10}b");

        Assert.Equal("a12345     b", vsb.ToString());
    }

    [Fact]
    public void IConvertibleを実装する型の右寄せのテスト()
    {
        using var vsb = new ValueStringBuilder(stackalloc char[10]);

        var value = new Convertible(12345);

        vsb.Append($"a{value,10}b");

        Assert.Equal("a     12345b", vsb.ToString());
    }
    

    [Fact]
    public void 変換インターフェイスを実装しない型のテスト()
    {
        using var vsb = new ValueStringBuilder(stackalloc char[10]);

        var value = new NoInterface(1234567890);

        vsb.Append($"a{value}b");

        Assert.Equal("a1234567890b", vsb.ToString());
    }
    

    [Fact]
    public void 変換インターフェイスを実装しない型の左寄せのテスト()
    {
        using var vsb = new ValueStringBuilder(stackalloc char[10]);

        var value = new NoInterface(12345);

        vsb.Append($"a{value,-10}b");

        Assert.Equal("a12345     b", vsb.ToString());
    }

    [Fact]
    public void 変換インターフェイスを実装しない型の右寄せのテスト()
    {
        using var vsb = new ValueStringBuilder(stackalloc char[10]);

        var value = new NoInterface(12345);

        vsb.Append($"a{value,10}b");

        Assert.Equal("a     12345b", vsb.ToString());
    }

    [Fact]
    public void ReadOnlySpanを足すテスト()
    {
        using var vsb = new ValueStringBuilder(stackalloc char[10]);

        var value = "12345".AsSpan();

        vsb.Append($"a{value}b");

        Assert.Equal("a12345b", vsb.ToString());
    }

    [Fact]
    public void ReadOnlySpanの左寄せのテスト()
    {
        using var vsb = new ValueStringBuilder(stackalloc char[10]);

        var value = "12345".AsSpan();

        vsb.Append($"a{value,-10}b");

        Assert.Equal("a12345     b", vsb.ToString());
    }

    [Fact]
    public void ReadOnlySpanの右寄せのテスト()
    {
        using var vsb = new ValueStringBuilder(stackalloc char[10]);

        var value = "12345".AsSpan();

        vsb.Append($"a{value,10}b");

        Assert.Equal("a     12345b", vsb.ToString());
    }
    

    [Fact]
    public void stringを足すテスト()
    {
        using var vsb = new ValueStringBuilder(stackalloc char[10]);

        var value = "12345";

        vsb.Append($"a{value}b");

        Assert.Equal("a12345b", vsb.ToString());
    }

    [Fact]
    public void stringの左寄せのテスト()
    {
        using var vsb = new ValueStringBuilder(stackalloc char[10]);

        var value = "12345";

        vsb.Append($"a{value,-10}b");

        Assert.Equal("a12345     b", vsb.ToString());
    }

    [Fact]
    public void stringの右寄せのテスト()
    {
        using var vsb = new ValueStringBuilder(stackalloc char[10]);

        var value = "12345";

        vsb.Append($"a{value,10}b");

        Assert.Equal("a     12345b", vsb.ToString());
    }

    [Fact]
    public void nullを足すテスト()
    {
        using var vsb = new ValueStringBuilder(stackalloc char[10]);

        object? obj = null;

        vsb.Append($"a{obj}b");

        Assert.Equal("ab", vsb.ToString());
    }

    private readonly struct SpanFormattable(int value) :
        ISpanFormattable
    {
        public int Value { get; } = value;

        public string ToString(
            string? format,
            IFormatProvider? formatProvider)
        {
            return this.Value.ToString(formatProvider);
        }

        public bool TryFormat(
            Span<char> destination,
            out int charsWritten,
            ReadOnlySpan<char> format,
            IFormatProvider? provider)
        {
            if (destination.Length < 100)
            {
                charsWritten = 0;
                return false;
            }

            return this.Value.TryFormat(destination, out charsWritten, format, provider);
        }
    }

    private readonly struct Formattable(int value) :
        IFormattable
    {
        public int Value { get; } = value;

        public string ToString(
            string? format,
            IFormatProvider? formatProvider)
        {
            return this.Value.ToString(format, formatProvider);
        }
    }

    private readonly struct Convertible(int value) :
        IConvertible
    {
        public int Value { get; } = value;

        public TypeCode GetTypeCode()
        {
            return TypeCode.Object;
        }

        public bool ToBoolean(IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }

        public byte ToByte(IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }

        public char ToChar(IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }

        public DateTime ToDateTime(IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }

        public decimal ToDecimal(IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }

        public double ToDouble(IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }

        public short ToInt16(IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }

        public int ToInt32(IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }

        public long ToInt64(IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }

        public sbyte ToSByte(IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }

        public float ToSingle(IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }

        public string ToString(IFormatProvider? provider)
        {
            return this.Value.ToString(provider);
        }

        public object ToType(Type conversionType, IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }

        public ushort ToUInt16(IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }

        public uint ToUInt32(IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }

        public ulong ToUInt64(IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }
    }

    private readonly struct NoInterface(int value)
    {
        public int Value { get; } = value;

        public override string ToString()
        {
            return this.Value.ToString();
        }
    }
}
