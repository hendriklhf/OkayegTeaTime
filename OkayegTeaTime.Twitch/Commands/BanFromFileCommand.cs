using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE.Strings;
using HLE.Twitch.Models;
using OkayegTeaTime.Database;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.BanFromFile, typeof(BanFromFileCommand))]
public readonly struct BanFromFileCommand(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    : IChatCommand<BanFromFileCommand>
{
    public PooledStringBuilder Response { get; } = new(AppSettings.MaxMessageLength);

    public IChatMessage ChatMessage { get; } = chatMessage;

    private readonly TwitchBot _twitchBot = twitchBot;
    private readonly ReadOnlyMemory<char> _prefix = prefix;
    private readonly ReadOnlyMemory<char> _alias = alias;

    private static readonly Regex _banPattern = new(@"^[\./]ban\s\w+", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1));

    public static void Create(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out BanFromFileCommand command)
    {
        command = new(twitchBot, chatMessage, prefix, alias);
    }

    public async ValueTask Handle()
    {
        Regex pattern = _twitchBot.MessageRegexCreator.Create(_alias.Span, _prefix.Span, @"\s\S+\s\S+");
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
                IChatMessage chatMessage = ChatMessage;
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

    public bool Equals(BanFromFileCommand other)
    {
        return _twitchBot.Equals(other._twitchBot) && _prefix.Equals(other._prefix) && _alias.Equals(other._alias) && Response.Equals(other.Response) && ChatMessage.Equals(other.ChatMessage);
    }

    public override bool Equals(object? obj)
    {
        return obj is BanFromFileCommand other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_twitchBot, _prefix, _alias, Response, ChatMessage);
    }

    public static bool operator ==(BanFromFileCommand left, BanFromFileCommand right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(BanFromFileCommand left, BanFromFileCommand right)
    {
        return !left.Equals(right);
    }
}
