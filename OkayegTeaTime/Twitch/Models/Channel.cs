using HLE.Strings;
using OkayegTeaTime.Database;

namespace OkayegTeaTime.Twitch.Models;

public class Channel
{
    public string Name
    {
        get => _name!;
        private init
        {
            _name = value;
            Database.Models.Channel? channel = DbController.GetChannel(value);
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
                DbController.UnsetEmoteInFront(Name);
            }
            else
            {
                DbController.SetEmoteInFront(Name, value);
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
                DbController.UnsetPrefix(Name);
            }
            else
            {
                DbController.SetPrefix(Name, value);
            }
        }
    }

    public bool IsEmoteSub
    {
        get => _isEmoteSub;
        set
        {
            _isEmoteSub = value;
            DbController.SetEmoteSub(Name, value);
        }
    }

    private readonly string? _name;
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
