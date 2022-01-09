using System.Text.RegularExpressions;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands.CommandClasses;

public class BanFromFileCommand : Command
{
    public BanFromFileCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Regex pattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s\S+\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            TwitchBot.Send(ChatMessage.Channel, Response);
        }
    }

    //public static void SendBanFromFile()
    //{
    //    try
    //    {
    //        if (AppSettings.UserLists.Moderators.Contains(chatMessage.UserId))
    //        {
    //            List<string> fileContent = new HttpGet(chatMessage.Split[1]).Result.Split("\n").ToList();
    //            string regex = chatMessage.Split[2];
    //            fileContent.Where(f => f.IsMatch(regex)).ForEach(f =>
    //            {
    //                if (f.IsMatch(@"^[\./]ban\s\w+"))
    //                {
    //                    twitchBot.TwitchClient.SendMessage(chatMessage.Channel.Name, f);
    //                }
    //                else
    //                {
    //                    twitchBot.TwitchClient.SendMessage(chatMessage.Channel.Name, $"/ban {f}");
    //                }
    //            });
    //        }
    //        else
    //        {
    //            twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, you must be a moderator of the bot");
    //        }
    //    }
    //    catch (Exception)
    //    {
    //        twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, something went wrong");
    //    }
    //}
}
