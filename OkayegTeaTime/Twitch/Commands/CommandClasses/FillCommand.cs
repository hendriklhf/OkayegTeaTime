using System.Text.RegularExpressions;
using HLE.Collections;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands.CommandClasses;

public class FillCommand : Command
{
    public FillCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Regex pattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            List<string> messageParts = new();
            string[] split = ChatMessage.Split[1..];
            int maxLength = AppSettings.MaxMessageLength - (ChatMessage.Channel.Emote.Length + 1);
            for (; ; )
            {
                string messagePart = split.Random();
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
