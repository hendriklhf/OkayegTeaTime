using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using HLE.Strings;

namespace OkayegTeaTime.Twitch.Helix;

[DebuggerDisplay("{ToString()}")]
[SuppressMessage("Design", "CA1001:Types that own disposable fields should be disposable", Justification = "it implements IDisposable")]
public struct UrlBuilder : IDisposable, IEquatable<UrlBuilder>
{
    public readonly ReadOnlySpan<char> WrittenSpan => _builder.WrittenSpan;

    public int ParameterCount { get; private set; }

    private readonly PooledStringBuilder _builder;

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
        _builder.Append(value);
        ParameterCount++;
    }

    [Pure]
    // ReSharper disable once ArrangeModifiersOrder
    public override readonly string ToString()
    {
        return _builder.ToString();
    }

    public readonly bool Equals(UrlBuilder other)
    {
        return _builder.Equals(other._builder) && ParameterCount == other.ParameterCount;
    }

    // ReSharper disable once ArrangeModifiersOrder
    public override readonly bool Equals(object? obj)
    {
        return obj is UrlBuilder other && Equals(other);
    }

    // ReSharper disable once ArrangeModifiersOrder
    public override readonly int GetHashCode()
    {
        return HashCode.Combine(_builder, ParameterCount);
    }

    public static bool operator ==(UrlBuilder left, UrlBuilder right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(UrlBuilder left, UrlBuilder right)
    {
        return !left.Equals(right);
    }
}
