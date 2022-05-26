using System.IO;

namespace OkayegTeaTime.Utils;

public class ArgsResolver
{
    public string[]? Channels { get; private set; }

    public string? SettingsPath { get; private set; }

    private readonly string[] _args;

    public ArgsResolver(string[] args)
    {
        _args = args;
    }

    public void Resolve()
    {
        GetChannels();
        GetSettingsPath();
    }

    private void GetChannels()
    {
        int idx = GetArgIdx("--channels");
        if (idx == -1)
        {
            return;
        }

        Regex pattern = new(@"^\w{3,25}(,\w{3,25})*");
        if (pattern.IsMatch(_args[idx + 1]))
        {
            Channels = pattern.Match(_args[idx + 1]).Value.Split(',');
        }
        else
        {
            throw new ArgumentException($"The \"channels\" argument (\"{_args[idx + 1]}\") at index {idx + 1} is in the wrong format. Expected: \"channel1,channel2,channel3\"");
        }
    }

    private void GetSettingsPath()
    {
        int idx = GetArgIdx("--settings");
        if (idx == -1)
        {
            return;
        }

        string args = string.Join(' ', _args[idx + 1]);
        Regex pattern = new("^\".+\"");
        if (!pattern.IsMatch(args))
        {
            return;
        }

        args = args[1..];
        int quoteIdx = args.IndexOf('"');
        string path = args[..quoteIdx];
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"The file ({path}) does not exist");
        }

        SettingsPath = path;
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
