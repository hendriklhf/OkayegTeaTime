using System.Text.RegularExpressions;
using HLE.Collections;
using HLE.Strings;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands.CommandClasses;

public class RandomWordCommand : Command
{
    public RandomWordCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        int count = 1;
        Regex countPattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s\d+");
        if (countPattern.IsMatch(ChatMessage.Message))
        {
            count = ChatMessage.Split[1].ToInt();
        }

        List<string> words = new();
        for (int i = 0; i < count; i++)
        {
            words.Add(AppSettings.RandomWords.Random());
        }
        Response = $"{ChatMessage.Username}, {words.JoinToString(' ')}";
        if (MessageHelper.IsMessageTooLong(Response.Message, ChatMessage.Channel))
        {
            Response = $"{Response.Message[..(AppSettings.MaxMessageLength - (3 + ChatMessage.Channel.Emote.Length + 1))]}...";
        }
    }
}
