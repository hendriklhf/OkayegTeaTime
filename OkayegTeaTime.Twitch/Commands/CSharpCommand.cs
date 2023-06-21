using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE.Twitch.Models;
using OkayegTeaTime.Database;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.CSharp, typeof(CSharpCommand))]
public readonly struct CSharpCommand : IChatCommand<CSharpCommand>
{
    public ResponseBuilder Response { get; }

    public ChatMessage ChatMessage { get; }

    private readonly TwitchBot _twitchBot;
    private readonly ReadOnlyMemory<char> _prefix;
    private readonly ReadOnlyMemory<char> _alias;

    public CSharpCommand(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    {
        ChatMessage = chatMessage;
        Response = new(AppSettings.MaxMessageLength);
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public static void Create(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out CSharpCommand command)
    {
        command = new(twitchBot, chatMessage, prefix, alias);
    }

    public async ValueTask Handle()
    {
        Regex pattern = _twitchBot.RegexCreator.Create(_alias.Span, _prefix.Span, @"\s.+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            using ChatMessageExtension messageExtension = new(ChatMessage);
            ReadOnlyMemory<char> mainMethodContent = ChatMessage.Message.AsMemory();
            mainMethodContent = mainMethodContent[(messageExtension.Split[0].Length + 1)..];
            Response.Append(ChatMessage.Username, ", ");

            try
            {
                DotNetFiddleResult dotNetFiddleResult = await _twitchBot.DotNetFiddleService.ExecuteCodeAsync(mainMethodContent);
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

    public void Dispose()
    {
        Response.Dispose();
    }
}
