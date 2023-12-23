using System.Diagnostics.Tracing;

namespace VSB;

[EventSource]
public sealed class ValueStringBuilderEventSource :
    EventSource
{
    private static class EventId
    {
        public const int Grown = 1;
        public const int Disposed = 2;
    }

    [Event(EventId.Grown, Level = EventLevel.Informational)]
    public void Grown()
    {
        this.WriteEvent(EventId.Grown);
    }

    [Event(EventId.Disposed, Level = EventLevel.Informational)]
    public void Disposed()
    {
        this.WriteEvent(EventId.Disposed);
    }

    public static readonly ValueStringBuilderEventSource Log = new();
}
