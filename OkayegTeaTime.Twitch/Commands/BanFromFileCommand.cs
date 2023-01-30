using System;
using System.Linq;
using System.Text.RegularExpressions;
using HLE;
using HLE.Collections;
using OkayegTeaTime.Database;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.BanFromFile)]
public readonly unsafe ref struct BanFromFileCommand
{
    public TwitchChatMessage ChatMessage { get; }

    public StringBuilder* Response { get; }

    private readonly TwitchBot _twitchBot;
    private readonly string? _prefix;
    private readonly string _alias;

    private static readonly Regex _banPattern = new(@"^[\./]ban\s\w+", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1));

    public BanFromFileCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        ChatMessage = chatMessage;
        Response = response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        Regex pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\s\S+\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            Response->Append(ChatMessage.Username, Messages.CommaSpace);
            try
            {
                if (!ChatMessage.IsBotModerator)
                {
                    Response->Append(Messages.YouArentAModeratorOfTheBot);
                    return;
                }

                HttpGet request = new(ChatMessage.Split[1]);
                if (request.Result is null)
                {
                    Response->Append("an error occurred while requesting the file content");
                    return;
                }

                string[] fileContent = request.Result.Replace("\r", string.Empty).Split("\n");
                Regex regex = new(ChatMessage.Split[2], RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1));
                TwitchBot twitchBot = _twitchBot;
                TwitchChatMessage chatMessage = ChatMessage;
                fileContent.Where(f => regex.IsMatch(f)).ForEach(f => twitchBot.Send(chatMessage.Channel, _banPattern.IsMatch(f) ? f : $"/ban {f}", false, false, false));
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
