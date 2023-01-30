using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using HLE;
using HLE.Collections;
using HLE.Emojis;
using OkayegTeaTime.Files;
using OkayegTeaTime.Resources;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Hangman)]
public readonly unsafe ref struct HangmanCommand
{
    public TwitchChatMessage ChatMessage { get; }

    public StringBuilder* Response { get; }

    private readonly TwitchBot _twitchBot;
    private readonly string? _prefix;
    private readonly string _alias;

    private static readonly string[] _hangmanWords = ResourceController.HangmanWords.Split(Environment.NewLine);
    private static readonly Dictionary<string, HangmanGame> _hangmanGames = new();

    public HangmanCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        ChatMessage = chatMessage;
        Response = response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        Regex pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\s[a-z]");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            if (!_hangmanGames.TryGetValue(ChatMessage.Channel, out HangmanGame? game))
            {
                Response->Append(ChatMessage.Username, Messages.CommaSpace, Messages.ThereIsNoGameRunningYouHaveToStartOneFirst);
                return;
            }

            char guess = ChatMessage.Message[_alias.Length + (_prefix?.Length ?? AppSettings.Suffix.Length) + 1];
            guess = char.ToLowerInvariant(guess);
            int correctPlacesCount = game.Guess(guess);
            Response->Append(ChatMessage.Username, Messages.CommaSpace);
            if (game.IsSolved)
            {
                Response->Append(Messages.TheGameHasBeenSolved, StringHelper.Whitespace, Emoji.PartyingFace, StringHelper.Whitespace);
                _hangmanGames.Remove(ChatMessage.Channel);
            }
            else if (game.WrongGuesses < HangmanGame.MaxWrongGuesses)
            {
                Response->Append('\'', guess, '\'', ' ');
                Span<char> buffer = stackalloc char[10];
                correctPlacesCount.TryFormat(buffer, out int bufferLength);
                Response->Append(Messages.IsCorrectIn, StringHelper.Whitespace, buffer[..bufferLength]);
                Response->Append(StringHelper.Whitespace, correctPlacesCount == 1 ? Messages.Places[..^1] : Messages.Places);
                Response->Append(correctPlacesCount > 0 ? Emoji.Grinning : Emoji.Cry, StringHelper.Whitespace);
            }
            else
            {
                Response->Append(Messages.TheMaximumWrongGuessesHaveBeenReachedTheSolutionWas, StringHelper.Whitespace, Messages.QuotationMark, game.Solution, Messages.QuotationMark);
                Response->Append(StringHelper.Whitespace, Emoji.Cry);
                _hangmanGames.Remove(ChatMessage.Channel);
                return;
            }

            AppendWordStatus(game);
            Response->Append(Messages.CommaSpace);
            AppendWrongCharStatus(game);
            Response->Append(Messages.CommaSpace);
            AppendGuessStatus(game);
            return;
        }

        pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\s[a-z]{2,}");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            if (!_hangmanGames.TryGetValue(ChatMessage.Channel, out HangmanGame? game))
            {
                Response->Append(ChatMessage.Username, Messages.CommaSpace, Messages.ThereIsNoGameRunningYouHaveToStartOneFirst);
                return;
            }

            string guess = ChatMessage.LowerSplit[1];
            bool correct = game.Guess(guess);
            Response->Append(ChatMessage.Username, Messages.CommaSpace);
            if (correct)
            {
                Response->Append(Messages.QuotationMark, game.Solution, Messages.QuotationMark);
                Response->Append(StringHelper.Whitespace, Messages.IsCorrectTheGameHasBeenSolved, StringHelper.Whitespace, Emoji.PartyingFace);
                _hangmanGames.Remove(ChatMessage.Channel);
            }
            else if (game.WrongGuesses < HangmanGame.MaxWrongGuesses)
            {
                Response->Append(Messages.QuotationMark, guess, Messages.QuotationMark, StringHelper.Whitespace, Messages.IsNotCorrect, StringHelper.Whitespace, Emoji.Cry);
                AppendWordStatus(game);
                Response->Append(Messages.CommaSpace);
                AppendWrongCharStatus(game);
                Response->Append(Messages.CommaSpace);
                AppendGuessStatus(game);
            }
            else
            {
                Response->Append(Messages.TheMaximumWrongGuessesHaveBeenReachedTheSolutionWas, StringHelper.Whitespace, Messages.QuotationMark, game.Solution, Messages.QuotationMark);
                Response->Append(StringHelper.Whitespace, Emoji.Cry);
                _hangmanGames.Remove(ChatMessage.Channel);
            }

            return;
        }

        pattern = _twitchBot.RegexCreator.Create(_alias, _prefix);
        if (pattern.IsMatch(ChatMessage.Message))
        {
            Response->Append(ChatMessage.Username, Messages.CommaSpace);
            if (_hangmanGames.TryGetValue(ChatMessage.Channel, out HangmanGame? game))
            {
                Response->Append(Messages.ThereIsAlreadyAGameRunning);
            }
            else
            {
                Response->Append(Messages.NewGameStarted);
                game = new(_hangmanWords.Random()!);
            }

            Response->Append(Messages.Colon, StringHelper.Whitespace);
            AppendWordStatus(game);

            _hangmanGames.Add(ChatMessage.Channel, game);
        }
    }

    private void AppendWordStatus(HangmanGame game)
    {
        Span<char> buffer = stackalloc char[100];
        int bufferLength = StringHelper.Join(game.DiscoveredWord, ' ', buffer);
        Response->Append(buffer[..bufferLength]);
        game.Solution.Length.TryFormat(buffer, out bufferLength);
        Response->Append(' ', '(');
        Response->Append(buffer[..bufferLength]);
        Response->Append(' ', 'c', 'h', 'a', 'r', 's', ')');
    }

    private void AppendWrongCharStatus(HangmanGame game)
    {
        Response->Append(Messages.WrongChars, Messages.Colon, StringHelper.Whitespace);
        Response->Append('[');
        Response->Append(game.WrongChars);
        Response->Append(']');
    }

    private void AppendGuessStatus(HangmanGame game)
    {
        Span<char> buffer = stackalloc char[10];
        game.WrongGuesses.TryFormat(buffer, out int bufferLength);
        Response->Append(buffer[..bufferLength]);
        Response->Append('/');
        HangmanGame.MaxWrongGuesses.TryFormat(buffer, out bufferLength);
        Response->Append(buffer[..bufferLength], StringHelper.Whitespace, Messages.WrongGuesses);
    }
}
