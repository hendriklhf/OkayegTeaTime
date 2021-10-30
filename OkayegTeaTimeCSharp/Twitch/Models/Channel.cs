using OkayegTeaTimeCSharp.Database;

namespace OkayegTeaTimeCSharp.Twitch.Models;

public class Channel
{
    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            _emote = DatabaseController.GetEmoteInFront(Name);
            _prefix = DatabaseController.GetPrefix(Name);
            _isEmoteSub = DatabaseController.IsEmoteManagementSub(Name);
        }
    }

    public string Emote
    {
        get => _emote ?? Config.DefaultEmote;
        set
        {
            _emote = value;
            if (value is null)
            {
                DatabaseController.UnsetEmoteInFront(Name);
            }
            else
            {
                DatabaseController.SetEmoteInFront(Name, Emote);
            }
        }
    }

    public string Prefix
    {
        get => _prefix ?? string.Empty;
        set
        {
            _prefix = value;
            if (value is null)
            {
                DatabaseController.UnsetPrefix(Name);
            }
            else
            {
                DatabaseController.SetPrefix(Name, Prefix);
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

    private string _name;
    private string _emote;
    private string _prefix;
    private bool _isEmoteSub;

    public Channel(string name)
    {
        Name = name.ToLower().RemoveHashtag();
    }

    public override string ToString()
    {
        return Name;
    }

    public override bool Equals(object obj)
    {
        return obj is Channel channel && channel.Name == Name;
    }

    public static implicit operator Channel(string channel)
    {
        return new(channel);
    }
}
