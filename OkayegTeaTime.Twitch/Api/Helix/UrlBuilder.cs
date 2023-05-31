using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using HLE.Strings;

namespace OkayegTeaTime.Twitch.Api.Helix;

[DebuggerDisplay("{ToString()}")]
internal struct UrlBuilder : IDisposable
{
    public readonly ReadOnlySpan<char> WrittenSpan => _builder.WrittenSpan;

    public int ParameterCount { get; private set; }

    private PoolBufferStringBuilder _builder;

    public UrlBuilder(ReadOnlySpan<char> baseUrl, ReadOnlySpan<char> endpoint, int initialBufferLength = 100)
    {
        _builder = new(initialBufferLength);
        _builder.Append(baseUrl);
        if (baseUrl[^1] != '/' && endpoint[0] != '/')
        {
            _builder.Append('/');
        }

        _builder.Append(endpoint);
    }

    public readonly void Dispose()
    {
        _builder.Dispose();
    }

    public void AppendParameter(ReadOnlySpan<char> key, ReadOnlySpan<char> value)
    {
        _builder.Append(ParameterCount == 0 ? '?' : '&');
        _builder.Append(key);
        _builder.Append('=');
        _builder.Append(value);
        ParameterCount++;
    }

    public void AppendParameter<T>(ReadOnlySpan<char> key, T value) where T : ISpanFormattable
    {
        _builder.Append(ParameterCount == 0 ? '?' : '&');
        _builder.Append(key);
        _builder.Append('=');
        _builder.Append<T, IFormatProvider>(value);
        ParameterCount++;
    }

    [Pure]
    // ReSharper disable once ArrangeModifiersOrder
    public override readonly string ToString()
    {
        return _builder.ToString();
    }
}
