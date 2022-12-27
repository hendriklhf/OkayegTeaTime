using System;

#pragma warning disable CS0660, CS0661

namespace OkayegTeaTime.Twitch.Models;

public ref struct Response
{
    public int Length { get; private set; }

    private readonly Span<char> _response = Span<char>.Empty;

    public Response()
    {
    }

    public Response(Span<char> responseBuffer)
    {
        _response = responseBuffer;
    }

    public void Append(scoped ReadOnlySpan<char> msg, scoped ReadOnlySpan<char> msg2 = default, scoped ReadOnlySpan<char> msg3 = default, scoped ReadOnlySpan<char> msg4 = default, scoped ReadOnlySpan<char> msg5 = default,
        scoped ReadOnlySpan<char> msg6 = default, scoped ReadOnlySpan<char> msg7 = default, scoped ReadOnlySpan<char> msg8 = default, scoped ReadOnlySpan<char> msg9 = default, scoped ReadOnlySpan<char> msg10 = default)
    {
        msg.CopyTo(_response[Length..]);
        Length += msg.Length;

        if (msg2 == default)
        {
            return;
        }

        msg2.CopyTo(_response[Length..]);
        Length += msg2.Length;

        if (msg3 == default)
        {
            return;
        }

        msg3.CopyTo(_response[Length..]);
        Length += msg3.Length;

        if (msg4 == default)
        {
            return;
        }

        msg4.CopyTo(_response[Length..]);
        Length += msg4.Length;

        if (msg5 == default)
        {
            return;
        }

        msg5.CopyTo(_response[Length..]);
        Length += msg5.Length;

        if (msg6 == default)
        {
            return;
        }

        msg6.CopyTo(_response[Length..]);
        Length += msg6.Length;

        if (msg7 == default)
        {
            return;
        }

        msg7.CopyTo(_response[Length..]);
        Length += msg7.Length;

        if (msg8 == default)
        {
            return;
        }

        msg8.CopyTo(_response[Length..]);
        Length += msg8.Length;

        if (msg9 == default)
        {
            return;
        }

        msg9.CopyTo(_response[Length..]);
        Length += msg9.Length;

        if (msg10 == default)
        {
            return;
        }

        msg10.CopyTo(_response[Length..]);
        Length += msg10.Length;
    }

    public static implicit operator string(Response response)
    {
        return response.ToString();
    }

    public static bool operator ==(Response left, Response right)
    {
        ReadOnlySpan<char> leftResponse = left._response[..left.Length];
        ReadOnlySpan<char> rightResponse = right._response[..right.Length];
        return leftResponse.Equals(rightResponse, StringComparison.Ordinal);
    }

    public static bool operator !=(Response left, Response right)
    {
        return !(left == right);
    }

    public readonly bool Equals(Response other)
    {
        return this == other;
    }

    public readonly override string ToString()
    {
        return new(_response[..Length]);
    }
}
