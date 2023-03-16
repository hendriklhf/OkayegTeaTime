using System;
using System.Linq;
using System.Text.RegularExpressions;
using HLE.Twitch;
using HLE.Twitch.Models;
using OkayegTeaTime.Database;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.BanFromFile)]
public readonly ref struct BanFromFileCommand
{
    public ChatMessage ChatMessage { get; }

    private readonly TwitchBot _twitchBot;
    private readonly ref MessageBuilder _response;
    private readonly ReadOnlySpan<char> _prefix;
    private readonly ReadOnlySpan<char> _alias;

    private static readonly Regex _banPattern = new(@"^[\./]ban\s\w+", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1));

    public BanFromFileCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        ChatMessage = chatMessage;
        _response = ref response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        Regex pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\s\S+\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            _response.Append(ChatMessage.Username, ", ");
            try
            {
                using ChatMessageExtension messageExtension = new(ChatMessage);
                if (!messageExtension.IsBotModerator)
                {
                    _response.Append(Messages.YouArentAModeratorOfTheBot);
                    return;
                }

                HttpGet request = new(new(messageExtension.Split[1]));
                if (request.Result is null)
                {
                    _response.Append("an error occurred while requesting the file content");
                    return;
                }

                string[] fileContent = request.Result.Replace("\r", string.Empty).Split("\n");
                Regex regex = new(new(messageExtension.Split[2]), RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1));
                TwitchBot twitchBot = _twitchBot;
                ChatMessage chatMessage = ChatMessage;
                foreach (string user in fileContent.Where(f => regex.IsMatch(f)))
                {
                    twitchBot.Send(chatMessage.Channel, _banPattern.IsMatch(user) ? user : $"/ban {user}", false, false, false);
                }

                _response.Append("done :)");
            }
            catch (Exception ex)
            {
                DbController.LogException(ex);
                _response.Append("something went wrong");
            }
        }
    }
}
