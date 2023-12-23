using Xunit;

namespace VSB.Tests;

public class DisposedExceptionTest
{
    [Fact]
    public void Dispose済みのオブジェクトにAppendすると死ぬテスト()
    {
        Assert.Throws<ObjectDisposedException>(() =>
        {
            var vsb = new ValueStringBuilder(stackalloc char[10]);

            vsb.Dispose();

            vsb.Append("a");
        });
    }
}