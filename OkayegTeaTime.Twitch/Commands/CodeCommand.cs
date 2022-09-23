using System;
using System.Linq;
using System.Text.RegularExpressions;
using HLE.Collections;
using OkayegTeaTime.Files;
using OkayegTeaTime.Resources;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Code)]
public sealed class CodeCommand : Command
{
    private static string[]? _codeFiles;

    public CodeCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias) : base(twitchBot, chatMessage, alias)
    {
        _codeFiles ??= ResourceController.CodeFiles.Split(new[]
        {
            "\r\n",
            "\n",
            "\r"
        }, StringSplitOptions.RemoveEmptyEntries);
    }

    public override void Handle()
    {
        Regex pattern = PatternCreator.Create(_alias, _prefix, @"\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            Regex? filePattern;
            try
            {
                filePattern = new(ChatMessage.Message[(ChatMessage.Split[0].Length + 1)..], RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));
            }
            catch (Exception)
            {
                Response = $"{ChatMessage.Username}, the given pattern is invalid";
                return;
            }

            string[] matchingFiles = _codeFiles!.Where(f => filePattern.IsMatch(f)).ToArray();
            Response = matchingFiles.Length switch
            {
                0 => $"{ChatMessage.Username}, your pattern matched no source code files",
                1 => $"{ChatMessage.Username}, {AppSettings.RepositoryUrl}/blob/master/{matchingFiles[0]}",
                <= 5 => $"{ChatMessage.Username}, your pattern matched {matchingFiles.Length} files: {matchingFiles.JoinToString(", ")}. Please specify",
                _ => $"{ChatMessage.Username}, your pattern matched too many ({matchingFiles.Length}) files. Please specify"
            };
        }
    }
}
