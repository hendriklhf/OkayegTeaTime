using System;
using System.IO;
using System.Text.RegularExpressions;

namespace OkayegTeaTime.Utils;

public class ArgsResolver
{
    public string[]? Channels { get; private set; }

    public string? SettingsPath { get; private set; }

    public string[]? ChannelsToExclude { get; private set; }

    private readonly string[] _args;
    private readonly Regex _channelListPattern = new(@"^\w{3,25}(,\w{3,25})*", RegexOptions.Compiled);

    public ArgsResolver(string[] args)
    {
        _args = args;
    }

    public void Resolve()
    {
        Channels = GetChannels();
        SettingsPath = GetSettingsPath();
        ChannelsToExclude = GetChannelsToExclude();
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
            return _args[idx + 1].Split(',');
        }

        throw new ArgumentException($"The \"channels\" argument (\"{_args[idx + 1]}\") at index {idx + 1} is in the wrong format. Expected: \"channel1,channel2,channel3\"");
    }

    private string? GetSettingsPath()
    {
        int idx = GetArgIdx("--settings");
        if (idx == -1)
        {
            return null;
        }

        string args = string.Join(' ', _args[idx + 1]);
        Regex pattern = new("^\".+\"");
        if (!pattern.IsMatch(args))
        {
            return null;
        }

        args = args[1..];
        int quoteIdx = args.IndexOf('"');
        string path = args[..quoteIdx];
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"The file ({path}) does not exist");
        }

        return path;
    }

    private string[]? GetChannelsToExclude()
    {
        int idx = GetArgIdx("--excluded-channels");
        if (idx == -1)
        {
            return null;
        }

        if (_channelListPattern.IsMatch(_args[idx + 1]))
        {
            return _args[idx + 1].Split(',');
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
