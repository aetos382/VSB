using System.Text;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using VSB;

BenchmarkRunner.Run<ValueStringBuilderBenchmark>();

[MemoryDiagnoser]
public class ValueStringBuilderBenchmark
{
    [Params(10, 100, 1000)]
    public int InitialLength { get; set; }

    [Params(1, 10, 100)]
    public int Iteration { get; set; }

    [Benchmark]
    public int UseStringBuilder()
    {
        var initialLength = this.InitialLength;
        var iteration = this.Iteration;

        var builder = new StringBuilder(initialLength);

        for (int i = 0; i < iteration; ++i)
        {
            builder.Append("ABC");
        }

        return builder.Length;
    }

    [Benchmark]
    public int UseValueStringBuilder()
    {
        var initialLength = this.InitialLength;
        var iteration = this.Iteration;

        using var builder = new ValueStringBuilder(stackalloc char[initialLength]);
        
        for (int i = 0; i < iteration; ++i)
        {
            builder.Append("ABC");
        }

        return builder.Length;
    }
}
