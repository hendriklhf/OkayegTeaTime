using System.Text.RegularExpressions;
using HLE.Collections;
using HLE.HttpRequests;
using HLE.Strings;
using OkayegTeaTime.Logging;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands.CommandClasses;

public class BanFromFileCommand : Command
{
    private static readonly Regex _banPattern = new(@"^[\./]ban\s\w+", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

    public BanFromFileCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Regex pattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s\S+\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            Response = $"{ChatMessage.Username}, ";
            try
            {
                if (!ChatMessage.IsBotModerator)
                {
                    Response += PredefinedMessages.NoBotModerator;
                    return;
                }

                HttpGet request = new(ChatMessage.Split[1]);
                if (request.Result is null)
                {
                    Response += "an error occurred while requesting the file content";
                    return;
                }

                List<string> fileContent = request.Result.Remove("\r").Split("\n").ToList();
                Regex regex = new(ChatMessage.Split[2], RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));
                fileContent.Where(f => regex.IsMatch(f)).ForEach(f =>
                {
                    if (_banPattern.IsMatch(f))
                    {
                        TwitchBot.TwitchClient.SendMessage(ChatMessage.Channel.Name, f);
                    }
                    else
                    {
                        TwitchBot.TwitchClient.SendMessage(ChatMessage.Channel.Name, $"/ban {f}");
                    }
                });
                Response += "done :)";
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                Response += "something went wrong";
            }
            return;
        }
    }
}
