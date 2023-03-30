using System.Linq;
using OkayegTeaTime.Database.EntityFrameworkModels;

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
            OkayegTeaTimeContext db = GetContext();
            try
            {
                EntityFrameworkModels.Channel? efChannel = db.Channels.FirstOrDefault(c => c.Id == Id);
                if (efChannel is null)
                {
                    return;
                }

                efChannel.EmoteInFront = value;
                EditedProperty();
            }
            finally
            {
                ReturnContext();
            }
        }
    }

    public string? Prefix
    {
        get => _prefix;
        set
        {
            _prefix = value;
            OkayegTeaTimeContext db = GetContext();
            try
            {
                EntityFrameworkModels.Channel? efChannel = db.Channels.FirstOrDefault(c => c.Id == Id);
                if (efChannel is null)
                {
                    return;
                }

                efChannel.Prefix = value;
                EditedProperty();
            }
            finally
            {
                ReturnContext();
            }
        }
    }

    private string? _emote;
    private string? _prefix;

    public Channel(EntityFrameworkModels.Channel channel)
    {
        Id = channel.Id;
        Name = channel.Name;
        _emote = channel.EmoteInFront;
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
