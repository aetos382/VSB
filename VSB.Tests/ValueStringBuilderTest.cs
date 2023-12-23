using System.Diagnostics.Tracing;
using Xunit;

namespace VSB.Tests;

public class ValueStringBuilderTest
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
}
