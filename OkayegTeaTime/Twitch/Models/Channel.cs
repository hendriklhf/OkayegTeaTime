using HLE.Strings;
using OkayegTeaTime.Database;

namespace OkayegTeaTime.Twitch.Models;

public class Channel
{
    public string Name
    {
        get => _name!;
        set
        {
            _name = value;
            Database.Models.Channel? channel = DatabaseController.GetChannel(value);
            _emote = channel?.EmoteInFront?.Decode() ?? AppSettings.DefaultEmote;
            _prefix = channel?.Prefix?.Decode();
            _isEmoteSub = channel?.EmoteManagementSub == true;
        }
    }

    public string Emote
    {
        get => _emote ?? AppSettings.DefaultEmote;
        set
        {
            _emote = value ?? AppSettings.DefaultEmote;
            if (value is null)
            {
                DatabaseController.UnsetEmoteInFront(Name);
            }
            else
            {
                DatabaseController.SetEmoteInFront(Name, value);
            }
        }
    }

    public string? Prefix
    {
        get => _prefix;
        set
        {
            _prefix = value;
            if (value is null)
            {
                DatabaseController.UnsetPrefix(Name);
            }
            else
            {
                DatabaseController.SetPrefix(Name, value);
            }
        }
    }

    public bool IsEmoteSub
    {
        get => _isEmoteSub;
        set
        {
            _isEmoteSub = value;
            DatabaseController.SetEmoteSub(Name, value);
        }
    }

    private string? _name;
    private string? _emote;
    private string? _prefix;
    private bool _isEmoteSub;

    public Channel(string name)
    {
        Name = name.ToLower().Remove("#");
    }

    public override string ToString()
    {
        return Name;
    }

    public override bool Equals(object? obj)
    {
        return obj is Channel channel && channel.Name == Name;
    }
}
