using HLE.Strings;
using OkayegTeaTime.Files.Jsons.HttpRequests.Bttv;
using OkayegTeaTime.Files.Jsons.HttpRequests.Ffz;
using OkayegTeaTime.Files.Jsons.HttpRequests.SevenTv;
using OkayegTeaTime.HttpRequests;

namespace OkayegTeaTime.Database.Models;

public class Channel : CacheModel
{
    public int Id { get; }

    public string Name { get; }

    public string? Emote
    {
        get => _emote;
        set
        {
            _emote = value;
            EntityFrameworkModels.Channel? efChannel = GetDbContext().Channels.FirstOrDefault(c => c.Id == Id);
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
            EntityFrameworkModels.Channel? efChannel = GetDbContext().Channels.FirstOrDefault(c => c.Id == Id);
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
            EntityFrameworkModels.Channel? efChannel = GetDbContext().Channels.FirstOrDefault(c => c.Id == Id);
            if (efChannel is null)
            {
                return;
            }

            efChannel.EmoteManagementSub = value;
            EditedProperty();
        }
    }

    public IEnumerable<string> Emotes => BttvEmotes.Concat(FfzEmotes).Concat(SevenTvEmotes);

    public IEnumerable<string> BttvEmotes
    {
        get
        {
            if (_bttvEmotes is not null)
            {
                return _bttvEmotes;
            }

            IEnumerable<BttvSharedEmote>? emotes = HttpRequest.GetBttvEmotes(Name);
            if (emotes is null)
            {
                _bttvEmotes = Array.Empty<string>();
                return _bttvEmotes;
            }

            _bttvEmotes = emotes.Select(e => e.Name).ToArray();
            return _bttvEmotes;
        }
    }

    public IEnumerable<string> FfzEmotes
    {
        get
        {
            if (_ffzEmotes is not null)
            {
                return _ffzEmotes;
            }

            IEnumerable<FfzEmote>? emotes = HttpRequest.GetFfzEmotes(Name);
            if (emotes is null)
            {
                _ffzEmotes = Array.Empty<string>();
                return _ffzEmotes;
            }

            _ffzEmotes = emotes.Select(e => e.Name).ToArray();
            return _ffzEmotes;
        }
    }

    public IEnumerable<string> SevenTvEmotes
    {
        get
        {
            if (_sevenTvEmotes is not null)
            {
                return _sevenTvEmotes;
            }

            IEnumerable<SevenTvEmote>? emotes = HttpRequest.GetSevenTvEmotes(Name);
            if (emotes is null)
            {
                _sevenTvEmotes = Array.Empty<string>();
                return _sevenTvEmotes;
            }

            _sevenTvEmotes = emotes.Select(e => e.Name).ToArray();
            return _sevenTvEmotes;
        }
    }

    private string? _emote;
    private string? _prefix;
    private bool _isEmoteNotificationSub;
    private string[]? _bttvEmotes;
    private string[]? _ffzEmotes;
    private string[]? _sevenTvEmotes;

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

    public Channel(int id, string name)
    {
        Id = id;
        Name = name;
        _emote = null;
        _prefix = null;
        _isEmoteNotificationSub = false;
    }
}
