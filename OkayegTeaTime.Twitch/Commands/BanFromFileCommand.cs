using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE.Twitch.Models;
using OkayegTeaTime.Database;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.BanFromFile, typeof(BanFromFileCommand))]
public readonly struct BanFromFileCommand : IChatCommand<BanFromFileCommand>
{
    public ResponseBuilder Response { get; }

    public ChatMessage ChatMessage { get; }

    private readonly TwitchBot _twitchBot;
    private readonly ReadOnlyMemory<char> _prefix;
    private readonly ReadOnlyMemory<char> _alias;

    private static readonly Regex _banPattern = new(@"^[\./]ban\s\w+", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1));

    public BanFromFileCommand(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    {
        ChatMessage = chatMessage;
        Response = new(AppSettings.MaxMessageLength);
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public static void Create(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out BanFromFileCommand command)
    {
        command = new(twitchBot, chatMessage, prefix, alias);
    }

    public async ValueTask Handle()
    {
        Regex pattern = _twitchBot.RegexCreator.Create(_alias.Span, _prefix.Span, @"\s\S+\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            Response.Append(ChatMessage.Username, ", ");
            try
            {
                using ChatMessageExtension messageExtension = new(ChatMessage);
                if (!messageExtension.IsBotModerator)
                {
                    Response.Append(Messages.YouArentAModeratorOfTheBot);
                    return;
                }

                HttpGet request = await HttpGet.GetStringAsync(new(messageExtension.Split[1].Span));
                if (request.Result is null)
                {
                    Response.Append("an error occurred while requesting the file content");
                    return;
                }

                string[] fileContent = request.Result.Replace("\r", string.Empty).Split("\n");
                Regex regex = new(new(messageExtension.Split[2].Span), RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1));
                TwitchBot twitchBot = _twitchBot;
                ChatMessage chatMessage = ChatMessage;
                foreach (string user in fileContent.Where(f => regex.IsMatch(f)))
                {
                    await twitchBot.SendAsync(chatMessage.Channel, _banPattern.IsMatch(user) ? user : $"/ban {user}", false, false, false);
                }

                Response.Append("done :)");
            }
            catch (Exception ex)
            {
                await DbController.LogExceptionAsync(ex);
                Response.Append("something went wrong");
            }
        }
    }

    public void Dispose()
    {
        Response.Dispose();
    }
}
