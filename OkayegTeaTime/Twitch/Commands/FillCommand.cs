using HLE.Collections;
using OkayegTeaTime.Database;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

public class FillCommand : Command
{
    public FillCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Regex pattern = PatternCreator.Create(Alias, Prefix, @"\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            List<string> messageParts = new();
            string[] split = ChatMessage.Split[1..];
            string emote = DbControl.Channels[ChatMessage.ChannelId]?.Emote ?? AppSettings.DefaultEmote;
            int maxLength = AppSettings.MaxMessageLength - (emote.Length + 1);
            for (; ; )
            {
                string? messagePart = split.Random();
                if (messagePart is null)
                {
                    break;
                }

                int currentMessageLength = messageParts.Sum(m => m.Length) + messageParts.Count + messagePart.Length;
                if (currentMessageLength <= maxLength)
                {
                    messageParts.Add(messagePart);
                }
                else
                {
                    break;
                }
            }

            Response = string.Join(' ', messageParts);
            return;
        }
    }
}
