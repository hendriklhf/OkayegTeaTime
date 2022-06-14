using System.Linq;
using HLE;
using OkayegTeaTime.Files;

namespace OkayegTeaTime.Database.Models;

public class Channel : CacheModel
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

            efChannel.EmoteInFront = value?.Encode();
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

            efChannel.Prefix = value?.Encode();
            EditedProperty();
        }
    }

    public bool IsEmoteNotificationSub
    {
        get => _isEmoteNotificationSub;
        set
        {
            _isEmoteNotificationSub = value;
            EntityFrameworkModels.Channel? efChannel = DbContext.Channels.FirstOrDefault(c => c.Id == Id);
            if (efChannel is null)
            {
                return;
            }

            efChannel.EmoteManagementSub = value;
            EditedProperty();
        }
    }

    private string? _emote;
    private string? _prefix;
    private bool _isEmoteNotificationSub;

    public Channel(EntityFrameworkModels.Channel channel)
    {
        Id = channel.Id;
        Name = channel.Name;

        string? emote = channel.EmoteInFront?.Decode();
        _emote = string.IsNullOrEmpty(emote) ? AppSettings.DefaultEmote : emote;

        string? prefix = channel.Prefix?.Decode();
        _prefix = string.IsNullOrEmpty(prefix) ? null : prefix;

        _isEmoteNotificationSub = channel.EmoteManagementSub == true;
    }

    public Channel(long id, string name)
    {
        Id = id;
        Name = name;
        _emote = null;
        _prefix = null;
        _isEmoteNotificationSub = false;
    }
}
