using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE.Strings;
using HLE.Twitch.Models;
using OkayegTeaTime.Database;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.CSharp, typeof(CSharpCommand))]
public readonly struct CSharpCommand(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    : IChatCommand<CSharpCommand>
{
    public PooledStringBuilder Response { get; } = new(GlobalSettings.MaxMessageLength);

    public IChatMessage ChatMessage { get; } = chatMessage;

    private readonly TwitchBot _twitchBot = twitchBot;
    private readonly ReadOnlyMemory<char> _prefix = prefix;
    private readonly ReadOnlyMemory<char> _alias = alias;

    public static void Create(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out CSharpCommand command)
        => command = new(twitchBot, chatMessage, prefix, alias);

    public async ValueTask Handle()
    {
        Regex pattern = _twitchBot.MessageRegexCreator.Create(_alias.Span, _prefix.Span, @"\s.+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            using ChatMessageExtension messageExtension = new(ChatMessage);
            ReadOnlyMemory<char> mainMethodContent = ChatMessage.Message.AsMemory();
            mainMethodContent = mainMethodContent[(messageExtension.Split[0].Length + 1)..];
            Response.Append(ChatMessage.Username, ", ");

            try
            {
                DotNetFiddleResult dotNetFiddleResult = await Services.DotNetFiddleService.ExecuteCodeAsync(mainMethodContent);
                ReadOnlyMemory<char> consoleOutput = dotNetFiddleResult.ConsoleOutput.AsMemory();
                if (consoleOutput.Length == 0)
                {
                    Response.Append("empty output, code executed successfully");
                }

                bool isConsoleOutputTooLong = consoleOutput.Length > 450;
                if (isConsoleOutputTooLong)
                {
                    consoleOutput = consoleOutput[..450];
                }

                Response.Append(consoleOutput.Span);
                if (isConsoleOutputTooLong)
                {
                    Response.Append("...");
                }
            }
            catch (Exception ex)
            {
                await DbController.LogExceptionAsync(ex);
                Response.Append(Messages.ApiError);
            }
        }
    }

    public void Dispose() => Response.Dispose();

    public bool Equals(CSharpCommand other) =>
        _twitchBot.Equals(other._twitchBot) && _prefix.Equals(other._prefix) && _alias.Equals(other._alias) &&
        Response.Equals(other.Response) && ChatMessage.Equals(other.ChatMessage);

    public override bool Equals(object? obj) => obj is CSharpCommand other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(_twitchBot, _prefix, _alias, Response, ChatMessage);

    public static bool operator ==(CSharpCommand left, CSharpCommand right) => left.Equals(right);

    public static bool operator !=(CSharpCommand left, CSharpCommand right) => !left.Equals(right);
}
