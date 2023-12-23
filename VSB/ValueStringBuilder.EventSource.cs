using System.Diagnostics.Tracing;

namespace VSB;

[EventSource]
public sealed class ValueStringBuilderEventSource :
    EventSource
{
    public ValueStringBuilderEventSource()
    {
        this._grownCounter = new IncrementingEventCounter(nameof(Grown), this);
    }

    [Event(EventId.Grown, Level = EventLevel.Informational)]
    public void Grown(
        int lengthToGrow)
    {
        this.WriteEvent(EventId.Grown, lengthToGrow);
        this._grownCounter.Increment(lengthToGrow);
    }

    [Event(EventId.Disposed, Level = EventLevel.Informational)]
    public void Disposed()
    {
        this.WriteEvent(EventId.Disposed);
    }

    protected override void Dispose(
        bool disposing)
    {
        if (disposing)
        {
            this._grownCounter.Dispose();
        }

        base.Dispose(disposing);
    }

    private readonly IncrementingEventCounter _grownCounter;

    public static class EventId
    {
        public const int Grown = 1;
        public const int Disposed = 2;
    }

    public static readonly ValueStringBuilderEventSource Log = new();
}
