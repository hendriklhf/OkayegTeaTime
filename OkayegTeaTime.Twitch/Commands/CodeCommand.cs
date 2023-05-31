using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE.Collections;
using HLE.Strings;
using HLE.Twitch.Models;
using OkayegTeaTime.Resources;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Code, typeof(CodeCommand))]
public readonly struct CodeCommand : IChatCommand<CodeCommand>
{
    public ResponseBuilder Response { get; }

    public ChatMessage ChatMessage { get; }

    private readonly TwitchBot _twitchBot;
    private readonly ReadOnlyMemory<char> _prefix;
    private readonly ReadOnlyMemory<char> _alias;

    private static string[]? _codeFiles;

    public CodeCommand(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    {
        ChatMessage = chatMessage;
        Response = new(AppSettings.MaxMessageLength);
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;

        _codeFiles ??= ResourceController.CodeFiles.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
    }

    public static void Create(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out CodeCommand command)
    {
        command = new(twitchBot, chatMessage, prefix, alias);
    }

    public ValueTask Handle()
    {
        Regex pattern = _twitchBot.RegexCreator.Create(_alias.Span, _prefix.Span, @"\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            Regex? filePattern;
            try
            {
                using ChatMessageExtension messageExtension = new(ChatMessage);
                filePattern = new(ChatMessage.Message[(messageExtension.Split[0].Length + 1)..], RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1));
            }
            catch (ArgumentException)
            {
                Response.Append(ChatMessage.Username, ", ", Messages.TheGivenPatternIsInvalid);
                return ValueTask.CompletedTask;
            }

            using PoolBufferList<string> matchingFiles = new(5);
            GetMatchingFiles(filePattern, matchingFiles);
            Response.Append(ChatMessage.Username, ", ");
            switch (matchingFiles.Count)
            {
                case 0:
                    Response.Append(Messages.YourPatternMatchedNoSourceCodeFiles);
                    break;
                case 1:
                    Response.Append(AppSettings.RepositoryUrl, "/blob/master/", matchingFiles[0]);
                    break;
                case <= 5:
                    Response.Append("your pattern matched ");
                    Response.Append(matchingFiles.Count);
                    Response.Append(" files: ");

                    int joinLength = StringHelper.Join(matchingFiles.AsSpan(), ", ", Response.FreeBufferSpan);
                    Response.Advance(joinLength);

                    Response.Append(". Please specify.");
                    break;
                default:
                    Response.Append("your pattern matched too many (");
                    Response.Append(matchingFiles.Count);
                    Response.Append(") files. Please specify.");
                    break;
            }
        }

        return ValueTask.CompletedTask;
    }

    private static void GetMatchingFiles<TCollection>(Regex filePattern, TCollection files) where TCollection : ICollection<string>
    {
        for (int i = 0; i < _codeFiles!.Length; i++)
        {
            string file = _codeFiles[i];
            if (filePattern.IsMatch(file))
            {
                files.Add(file);
            }
        }
    }

    public void Dispose()
    {
        Response.Dispose();
    }
}
