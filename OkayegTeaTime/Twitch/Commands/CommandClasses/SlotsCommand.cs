using HLE.Collections;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands.CommandClasses;

public class SlotsCommand : Command
{
    private const byte _emoteSlotCount = 3;

    public SlotsCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Regex? emotePattern = null;
        Regex pattern = PatternCreator.Create(Alias, Prefix, @"\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            try
            {
                emotePattern = new(ChatMessage.Split[1], RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));
            }
            catch (ArgumentException)
            {
                Response = $"{ChatMessage.Username}, you provided pattern is invalid";
                return;
            }
        }

        Channel? channel = DbControl.Channels[ChatMessage.ChannelId];
        if (channel is null)
        {
            Response = $"{ChatMessage.Username}, unable to retrieve channel information";
            return;
        }

        string[] emotes = channel.Emotes.ToArray();
        if (emotes.Length == 0)
        {
            Response = $"{ChatMessage.Username}, there are no third party emotes enabled in this channel";
            return;
        }

        if (emotePattern is not null)
        {
            emotes = emotes.Where(e => emotePattern.IsMatch(e)).ToArray();
        }

        if (emotes.Length == 0)
        {
            Response = $"{ChatMessage.Username}, there is no emote matching your provided pattern";
            return;
        }

        string[] randomEmotes = new string[_emoteSlotCount];
        for (int i = 0; i < _emoteSlotCount; i++)
        {
            randomEmotes[i] = emotes.Random()!;
        }

        string msgEmotes = string.Join(' ', randomEmotes);
        Response = $"{ChatMessage.Username}, [ {msgEmotes} ] ({emotes.Length} emotes)";
    }
}
