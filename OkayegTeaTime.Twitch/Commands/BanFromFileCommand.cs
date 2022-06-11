using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HLE;
using HLE.Collections;
using HLE.Http;
using OkayegTeaTime.Database;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

public class BanFromFileCommand : Command
{
    private static readonly Regex _banPattern = new(@"^[\./]ban\s\w+", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

    public BanFromFileCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Regex pattern = PatternCreator.Create(Alias, Prefix, @"\s\S+\s\S+");
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
                fileContent.Where(f => regex.IsMatch(f)).ForEach(f => _twitchBot.SendText(ChatMessage.Channel, _banPattern.IsMatch(f) ? f : $"/ban {f}"));
                Response += "done :)";
            }
            catch (Exception ex)
            {
                DbController.LogException(ex);
                Response += "something went wrong";
            }
        }
    }
}
