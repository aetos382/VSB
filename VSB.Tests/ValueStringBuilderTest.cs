using Xunit;

namespace VSB.Tests;

public class ValueStringBuilderTest
{
    [Fact]
    public void Q()
    {
        using var vsb = new ValueStringBuilder(stackalloc char[10]);
        
        Assert.Equal(0, vsb.Length);

        vsb.Append("ABC");

        Assert.Equal(3, vsb.Length);
        Assert.Equal("ABC", vsb.ToString());

        Span<char> span = stackalloc char[3];

        bool result = vsb.TryCopyTo(span);

        Assert.True(result);
        Assert.True("ABC".AsSpan().SequenceEqual(span));

        vsb.Append($"DEF{10,10:x}DEF");

        Assert.Equal(19, vsb.Length);
        Assert.Equal("ABCDEF         aDEF", vsb.ToString());
    }
}
