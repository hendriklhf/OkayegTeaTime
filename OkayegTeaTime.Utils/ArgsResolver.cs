using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace OkayegTeaTime.Utils;

public readonly partial struct ArgsResolver : IEquatable<ArgsResolver>
{
    public ReadOnlyMemory<string> Channels => _channels;

    private readonly string[] _args;
    private readonly string[]? _channels;
    private readonly Regex _channelListPattern = GetChannelListPattern();

    public ArgsResolver(string[] args)
    {
        _args = args;
        _channels = GetChannels();
    }

    [GeneratedRegex(@"^\w{3,25}(,\w{3,25})*", RegexOptions.Compiled)]
    private static partial Regex GetChannelListPattern();

    private string[]? GetChannels()
    {
        int idx = GetArgumentIndex("--channels");
        if (idx == -1)
        {
            return null;
        }

        if (_channelListPattern.IsMatch(_args[idx + 1]))
        {
            return _args[idx + 1].Split(',').Select(static c => c.ToLowerInvariant()).ToArray();
        }

        throw new ArgumentException($"The \"channels\" argument (\"{_args[idx + 1]}\") at index {idx + 1} is in the wrong format. Expected: \"channel1,channel2,channel3\"");
    }

    private int GetArgumentIndex(string argumentName)
    {
        for (int i = 0; i < _args.Length; i++)
        {
            if (_args[i] == argumentName)
            {
                return i;
            }
        }

        return -1;
    }

    public bool Equals(ArgsResolver other) =>
        _args == other._args && _channelListPattern == other._channelListPattern && Channels.Span.SequenceEqual(other.Channels.Span);

    public override bool Equals(object? obj) => obj is ArgsResolver other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(_args, _channelListPattern, _channels);

    public static bool operator ==(ArgsResolver left, ArgsResolver right) => left.Equals(right);

    public static bool operator !=(ArgsResolver left, ArgsResolver right) => !left.Equals(right);
}
