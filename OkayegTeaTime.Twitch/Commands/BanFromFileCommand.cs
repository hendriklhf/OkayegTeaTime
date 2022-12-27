using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HLE;
using HLE.Collections;
using HLE.Http;
using OkayegTeaTime.Database;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.BanFromFile)]
public readonly unsafe ref struct BanFromFileCommand
{
    public TwitchChatMessage ChatMessage { get; }

    public Response* Response { get; }

    private readonly TwitchBot _twitchBot;
    private readonly string? _prefix;
    private readonly string _alias;

    private static readonly Regex _banPattern = new(@"^[\./]ban\s\w+", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

    public BanFromFileCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, Response* response, string? prefix, string alias)
    {
        ChatMessage = chatMessage;
        Response = response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        Regex pattern = PatternCreator.Create(_alias, _prefix, @"\s\S+\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace);
            try
            {
                if (!ChatMessage.IsBotModerator)
                {
                    Response->Append(PredefinedMessages.YouArentAModeratorOfTheBot);
                    return;
                }

                HttpGet request = new(ChatMessage.Split[1]);
                if (request.Result is null)
                {
                    Response->Append("an error occurred while requesting the file content");
                    return;
                }

                List<string> fileContent = request.Result.Remove("\r").Split("\n").ToList();
                Regex regex = new(ChatMessage.Split[2], RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));
                TwitchBot twitchBot = _twitchBot;
                TwitchChatMessage chatMessage = ChatMessage;
                fileContent.Where(f => regex.IsMatch(f)).ForEach(f => twitchBot.SendText(chatMessage.Channel, _banPattern.IsMatch(f) ? f : $"/ban {f}"));
                Response->Append("done :)");
            }
            catch (Exception ex)
            {
                DbController.LogException(ex);
                Response->Append("something went wrong");
            }
        }
    }
}
