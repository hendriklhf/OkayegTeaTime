using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE.Emojis;
using HLE.Twitch.Models;
using OkayegTeaTime.Configuration;

namespace OkayegTeaTime.Twitch.Handlers;

public sealed class PajaAlertHandler(TwitchBot twitchBot) : PajaHandler(twitchBot)
{
    protected override Regex Pattern { get; } = Utils.Pattern.PajaAlert;

    protected override string Message => $"/me pajaStare {Emoji.RotatingLight} OBACHT";

    public override async ValueTask HandleAsync(IChatMessage chatMessage)
    {
        if (chatMessage.ChannelId != PajaChannelId || chatMessage.UserId != PajaAlertUserId || !Pattern.IsMatch(chatMessage.Message))
        {
            return;
        }

        await _twitchBot.SendAsync(PajaAlertChannel, Message, false, false, false);
        await _twitchBot.SendAsync(GlobalSettings.Settings.OfflineChat!.Channel, $"{GlobalSettings.DefaultEmote} {Emoji.RotatingLight}", false, false, false);
    }
}
