using System.Linq;
using OkayegTeaTime.Files;

namespace OkayegTeaTime.Database.Models;

public sealed class Channel : CacheModel
{
    public long Id { get; }

    public string Name { get; }

    public string? Emote
    {
        get => _emote;
        set
        {
            _emote = value;
            EntityFrameworkModels.Channel? efChannel = DbContext.Channels.FirstOrDefault(c => c.Id == Id);
            if (efChannel is null)
            {
                return;
            }

            efChannel.EmoteInFront = value;
            EditedProperty();
        }
    }

    public string? Prefix
    {
        get => _prefix;
        set
        {
            _prefix = value;
            EntityFrameworkModels.Channel? efChannel = DbContext.Channels.FirstOrDefault(c => c.Id == Id);
            if (efChannel is null)
            {
                return;
            }

            efChannel.Prefix = value;
            EditedProperty();
        }
    }

    private string? _emote;
    private string? _prefix;

    public Channel(EntityFrameworkModels.Channel channel)
    {
        Id = channel.Id;
        Name = channel.Name;

        string? emote = channel.EmoteInFront;
        _emote = string.IsNullOrWhiteSpace(emote) ? AppSettings.DefaultEmote : emote;

        _prefix = channel.Prefix;
    }

    public Channel(long id, string name)
    {
        Id = id;
        Name = name;
        _emote = null;
        _prefix = null;
    }
}
