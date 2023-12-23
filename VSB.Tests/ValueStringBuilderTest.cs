using System.Diagnostics.Tracing;
using Xunit;

namespace VSB.Tests;

public sealed class ValueStringBuilderTest
{
    [Fact]
    public void 初期化直後の状態のテスト()
    {
        using var vsb = new ValueStringBuilder(stackalloc char[10]);
        
        Assert.Equal(0, vsb.Length);

        var str = vsb.ToString();

        Assert.Equal(string.Empty, str);

        var array = vsb.ToArray();

        Assert.Equal(Array.Empty<char>(), array);
    }

    [Fact]
    public void 伸長が発生しないケースのテスト()
    {
        int count = 0;
        int length = 0;

        using var eventListener = new ValueStringBuilderEventListener();

        eventListener.Grown += (_, e) =>
        {
            ++count;
            length += e.Length;
        };

        using var vsb = new ValueStringBuilder(stackalloc char[10]);
        
        vsb.Append("abcdefghij");
                
        Assert.Equal(10, vsb.Length);

        var str = vsb.ToString();

        Assert.Equal("abcdefghij", str);

        var array = vsb.ToArray();

        Assert.Equal("abcdefghij".ToCharArray(), array);

        Assert.Equal(0, count);
        Assert.Equal(0, length);
    }

    [Fact]
    public void 伸長が発生するケースのテスト()
    {
        int count = 0;
        int length = 0;

        using var eventListener = new ValueStringBuilderEventListener();

        eventListener.Grown += (_, e) =>
        {
            ++count;
            length += e.Length;
        };

        using var vsb = new ValueStringBuilder(stackalloc char[10]);
        
        vsb.Append("abcdefghijklm");
                
        Assert.Equal(13, vsb.Length);

        var str = vsb.ToString();

        Assert.Equal("abcdefghijklm", str);

        var array = vsb.ToArray();

        Assert.Equal("abcdefghijklm".ToCharArray(), array);
        
        Assert.NotEqual(0, count);
        Assert.NotEqual(0, length);
    }

    [Fact]
    public void 値が既にある状態で伸長が発生するケースのテスト()
    {
        using var vsb = new ValueStringBuilder(stackalloc char[10]);
        
        vsb.Append("abcdefghijklm");
        vsb.Append("nopqrstuvwxyz");
                
        Assert.Equal(26, vsb.Length);

        var str = vsb.ToString();

        Assert.Equal("abcdefghijklmnopqrstuvwxyz", str);

        var array = vsb.ToArray();

        Assert.Equal("abcdefghijklmnopqrstuvwxyz".ToCharArray(), array);
    }

    [Fact]
    public void Clearのテスト()
    {
        using var vsb = new ValueStringBuilder(stackalloc char[10]);

        vsb.Append("abc");

        Assert.Equal(3, vsb.Length);
        Assert.Equal("abc", vsb.ToString());

        vsb.Clear();

        Assert.Equal(0, vsb.Length);
        Assert.Equal(string.Empty, vsb.ToString());
    }

    [Fact]
    public void nullを足すテスト()
    {
        using var vsb = new ValueStringBuilder(stackalloc char[10]);

        vsb.Append(null);

        Assert.Equal(0, vsb.Length);
        Assert.Equal(string.Empty, vsb.ToString());
    }

    [Fact]
    public void ReadOnlySpanを足すテスト()
    {
        using var vsb = new ValueStringBuilder(stackalloc char[10]);

        vsb.Append("abc".AsSpan());

        Assert.Equal(3, vsb.Length);
        Assert.Equal("abc", vsb.ToString());
    }
    

    [Fact]
    public void 空のReadOnlySpanを足すテスト()
    {
        using var vsb = new ValueStringBuilder(stackalloc char[10]);

        vsb.Append([]);

        Assert.Equal(0, vsb.Length);
        Assert.Equal(string.Empty, vsb.ToString());
    }

    [Fact]
    public void 一文字足すテスト()
    {
        using var vsb = new ValueStringBuilder(stackalloc char[10]);

        vsb.Append('a');

        Assert.Equal(1, vsb.Length);
        Assert.Equal("a", vsb.ToString());
    }

    [Fact]
    public void TryCopyToのテスト()
    {
        using var vsb = new ValueStringBuilder(stackalloc char[10]);

        vsb.Append("abcdefghij");

        Span<char> destination = stackalloc char[10];

        var result = vsb.TryCopyTo(destination);

        Assert.True(result);
        Assert.Equal("abcdefghij".ToCharArray(), destination.ToArray());
    }

    [Fact]
    public void TryCopyTo_int_intのテスト()
    {
        using var vsb = new ValueStringBuilder(stackalloc char[10]);

        vsb.Append("abcdefghij");

        Span<char> destination = stackalloc char[10];

        var result = vsb.TryCopyTo(destination, 3, 3);

        Assert.True(result);
        Assert.Equal("def".ToCharArray(), destination.Slice(0, 3).ToArray());
    }

    [Fact]
    public void TryCopyTo_Rangeのテスト()
    {
        using var vsb = new ValueStringBuilder(stackalloc char[10]);

        vsb.Append("abcdefghij");

        Span<char> destination = stackalloc char[10];

        var result = vsb.TryCopyTo(destination, 3..6);

        Assert.True(result);
        Assert.Equal("def".ToCharArray(), destination.Slice(0, 3).ToArray());
    }

    [Fact]
    public void SetLengthによって伸長が発生するテスト()
    {
        using var vsb = new ValueStringBuilder(stackalloc char[5]);

        vsb.SetLength(10);

        Assert.Equal(new char[10], vsb.ToArray());
    }
    

    [Fact]
    public void SetLengthに負の値を設定すると死ぬテスト()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            using var vsb = new ValueStringBuilder(stackalloc char[10]);

            vsb.SetLength(-1);
        });
    }

    [Fact]
    public void GetMemoryは残念ながらサポートしていない()
    {
        using var vsb = new ValueStringBuilder(stackalloc char[10]);

        var writer = vsb.GetWriter();

        Assert.Throws<NotSupportedException>(() => writer.GetMemory(10));
    }

    [Fact]
    public void Disposeを2回しても大丈夫()
    {
        var vsb = new ValueStringBuilder(stackalloc char[10]);

        vsb.Dispose();
        vsb.Dispose();
    }
}
