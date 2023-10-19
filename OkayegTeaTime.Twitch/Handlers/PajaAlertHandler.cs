using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE.Emojis;
using HLE.Twitch.Models;
using OkayegTeaTime.Settings;

namespace OkayegTeaTime.Twitch.Handlers;

public sealed class PajaAlertHandler(TwitchBot twitchBot) : PajaHandler(twitchBot)
{
    protected override Regex Pattern { get; } = Utils.Pattern.PajaAlert;

    protected override string Message => $"/me pajaStare {Emoji.RotatingLight} OBACHT";

    public override async ValueTask HandleAsync(IChatMessage chatMessage)
    {
        if (chatMessage.ChannelId != _pajaChannelId || chatMessage.UserId != _pajaAlertUserId || !Pattern.IsMatch(chatMessage.Message))
        {
            return;
        }

        Task sendAlertTask = _twitchBot.SendAsync(_pajaAlertChannel, Message, false, false, false).AsTask();
        Task sendOtherAlertTask = _twitchBot.SendAsync(GlobalSettings.Settings.OfflineChat!.Channel, $"{GlobalSettings.DefaultEmote} {Emoji.RotatingLight}", false, false, false).AsTask();
        await Task.WhenAll(sendAlertTask, sendOtherAlertTask);
    }
}
