using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace OkayegTeaTime.Utils;

public readonly ref struct ArgsResolver
{
    public string[]? Channels { get; }

    private readonly Span<string> _args;

    private static readonly Regex _channelListPattern = new(@"^\w{3,25}(,\w{3,25})*", RegexOptions.Compiled);

    public ArgsResolver(Span<string> args)
    {
        _args = args;
        Channels = GetChannels();
    }

    private string[]? GetChannels()
    {
        int idx = GetArgIdx("--channels");
        if (idx == -1)
        {
            return null;
        }

        if (_channelListPattern.IsMatch(_args[idx + 1]))
        {
            return _args[idx + 1].Split(',').Select(c => c.ToLower()).ToArray();
        }

        throw new ArgumentException($"The \"channels\" argument (\"{_args[idx + 1]}\") at index {idx + 1} is in the wrong format. Expected: \"channel1,channel2,channel3\"");
    }

    private int GetArgIdx(string argumentName)
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
}
