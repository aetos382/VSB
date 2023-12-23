using System.Diagnostics.Tracing;

namespace VSB.Tests;

internal sealed class ValueStringBuilderEventListener :
    EventListener
{
    protected override void OnEventSourceCreated(
        EventSource eventSource)
    {
        if (eventSource is ValueStringBuilderEventSource)
        {
            this.EnableEvents(
                eventSource,
                EventLevel.LogAlways,
                EventKeywords.All,
                new Dictionary<string, string?>
                {
                    ["EventCounterIntervalSec"] = "1"
                });
        }
    }

    protected override void OnEventWritten(
        EventWrittenEventArgs eventData)
    {
        if (eventData.EventSource is not ValueStringBuilderEventSource)
        {
            return;
        }

        switch (eventData.EventId)
        {
            case ValueStringBuilderEventSource.EventId.Grown:
                this.Grown?.Invoke(this, new GrownEventArgs((int)eventData.Payload![0]!));
                return;

            case ValueStringBuilderEventSource.EventId.Disposed:
                this.Disposed?.Invoke(this, EventArgs.Empty);
                return;
        }
    }

    public sealed class GrownEventArgs(int length) :
        EventArgs
    {
        public int Length { get; } = length;
    }

    public event EventHandler<GrownEventArgs>? Grown;

    public event EventHandler? Disposed;
}
