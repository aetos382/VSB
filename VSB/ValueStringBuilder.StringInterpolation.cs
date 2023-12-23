using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace VSB;

public ref partial struct ValueStringBuilder
{
    public void Append(
        [InterpolatedStringHandlerArgument("")] InterpolatedStringHandler handler)
    {
        this.Append(null, handler);
    }

    public void Append(
        IFormatProvider? formatProvider,
        [InterpolatedStringHandlerArgument("", nameof(formatProvider))] InterpolatedStringHandler handler)
    {
    }

    [InterpolatedStringHandler]
    public readonly ref struct InterpolatedStringHandler
    {
        public InterpolatedStringHandler(
            int literalLength,
            int placeholderCount,
            ValueStringBuilder receiver,
            IFormatProvider? formatProvider = null)
        {
            this._builder = receiver;
            this._formatProvider = formatProvider;
        }

        public void AppendLiteral(
            string value)
        {
            ArgumentNullException.ThrowIfNull(value);

            this.AppendChars(value.AsSpan());
        }

        public void AppendFormatted<T>(
            T? value,
            int alignment = 0,
            string? format = null)
        {
            if (value is null)
            {
                return;
            }

            if (value is string s)
            {
                this.AppendAlignedChars(s.AsSpan(), alignment);
                return;
            }

            if (value is ISpanFormattable sf)
            {
                this.AppendSpanFormattable(sf, alignment, format);
                return;
            }

            var formatProvider = this._formatProvider;

            var str = value switch
            {
                IFormattable f => f.ToString(format, formatProvider),
                IConvertible c => c.ToString(formatProvider),
                _ => ConvertToString(value, formatProvider)
            };

            this.AppendAlignedChars(str.AsSpan(), alignment);

            static string ConvertToString(
                object value,
                IFormatProvider? formatProvider)
            {
                var converter = TypeDescriptor.GetConverter(value);
                var cultureInfo = (formatProvider as CultureInfo) ?? CultureInfo.CurrentCulture;

                return converter.ConvertToString(null, cultureInfo, value)!;
            }
        }

        public void AppendFormatted(
            ReadOnlySpan<char> value,
            int alignment = 0)
        {
            this.AppendAlignedChars(value, alignment);
        }

        private void AppendSpanFormattable(
            ISpanFormattable value,
            int alignment,
            string? format)
        {
            var writer = this._builder.GetWriter();

            var minimumLength = Math.Abs(alignment);
            var bufferSize = Math.Max(minimumLength, 10);
            var buffer = writer.GetSpan(bufferSize);

            int charsWritten = 0;

            while (true)
            {
                if (value.TryFormat(buffer, out charsWritten, format, this._formatProvider))
                {
                    break;
                }

                bufferSize *= 2;
                buffer = writer.GetSpan(bufferSize);
            }

            Debug.Assert(bufferSize >= minimumLength);

            var fill = Math.Max(0, minimumLength - charsWritten);
            if (fill > 0)
            {
                Debug.Assert(alignment != 0);

                if (alignment < 0)
                {
                    buffer.Slice(charsWritten, fill).Fill(' ');
                }
                else
                {
                    buffer.Slice(0, charsWritten).CopyTo(buffer.Slice(fill));
                    buffer.Slice(0, fill).Fill(' ');
                }
            }
            
            writer.Advance(Math.Max(minimumLength, charsWritten));
        }

        private void AppendAlignedChars(
            ReadOnlySpan<char> value,
            int alignment)
        {
            var length = value.Length;

            var minimumLength = Math.Abs(alignment);
            var totalLength = Math.Max(value.Length, minimumLength);
            var fill = Math.Max(minimumLength - length, 0);

            var writer = this._builder.GetWriter();
            var buffer = writer.GetSpan(totalLength);

            if (fill > 0 && alignment > 0)
            {
                buffer.Slice(0, fill).Fill(' ');
                buffer = buffer.Slice(fill);
            }

            value.CopyTo(buffer);
            buffer = buffer.Slice(length);

            if (fill > 0 && alignment < 0)
            {
                buffer.Slice(0, fill).Fill(' ');
            }

            writer.Advance(totalLength);
        }

        private void AppendChars(
            scoped ReadOnlySpan<char> value)
        {
            this._builder.AppendNoCheck(value);
        }

        private readonly ValueStringBuilder _builder;

        private readonly IFormatProvider? _formatProvider;
    }
}
