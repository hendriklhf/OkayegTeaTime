using System;
using System.Text.RegularExpressions;
using HLE;
using HLE.Collections;
using HLE.Emojis;
using HLE.Twitch;
using HLE.Twitch.Models;
using OkayegTeaTime.Resources;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Hangman)]
public readonly ref struct HangmanCommand
{
    public ChatMessage ChatMessage { get; }

    private readonly TwitchBot _twitchBot;
    private readonly ref MessageBuilder _response;
    private readonly ReadOnlySpan<char> _prefix;
    private readonly ReadOnlySpan<char> _alias;

    private readonly ReadOnlySpan<char> _happyEmote;
    private readonly ReadOnlySpan<char> _sadEmote;
    private readonly ReadOnlySpan<char> _partyEmote;

    private static readonly string[] _hangmanWords = ResourceController.HangmanWords.Split("\r\n");

    public HangmanCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        ChatMessage = chatMessage;
        _response = ref response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;

        _happyEmote = _twitchBot.EmoteController.GetEmote(chatMessage.ChannelId, Emoji.Grinning, "happy", "good");
        _sadEmote = _twitchBot.EmoteController.GetEmote(chatMessage.ChannelId, Emoji.Cry, "cry", "sad", "bad");
        _partyEmote = _twitchBot.EmoteController.GetEmote(chatMessage.ChannelId, Emoji.PartyingFace, "cheer", "happy", "good");
    }

    public void Handle()
    {
        Regex pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\s[a-z]");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            GuessChar();
            return;
        }

        pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\s[a-z]{2,}");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            GuessWord();
            return;
        }

        pattern = _twitchBot.RegexCreator.Create(_alias, _prefix);
        if (pattern.IsMatch(ChatMessage.Message))
        {
            StartNewGame();
        }
    }

    private void StartNewGame()
    {
        _response.Append(ChatMessage.Username, ", ");
        if (_twitchBot.HangmanGames.TryGetValue(ChatMessage.ChannelId, out HangmanGame? game))
        {
            _response.Append(Messages.ThereIsAlreadyAGameRunning);
        }
        else
        {
            _response.Append(Messages.NewGameStarted);
            game = new(_hangmanWords.Random()!);
        }

        _response.Append(":", " ");
        AppendWordStatus(game);
        _twitchBot.HangmanGames.Add(ChatMessage.ChannelId, game);
    }

    private void GuessWord()
    {
        if (!_twitchBot.HangmanGames.TryGetValue(ChatMessage.ChannelId, out HangmanGame? game))
        {
            _response.Append(ChatMessage.Username, ", ", Messages.ThereIsNoGameRunningYouHaveToStartOneFirst);
            return;
        }

        using ChatMessageExtension messageExtension = new(ChatMessage);
        ReadOnlySpan<char> guess = messageExtension.LowerSplit[1];
        bool isWordCorrect = game.Guess(guess);
        _response.Append(ChatMessage.Username, ", ");
        if (isWordCorrect)
        {
            _response.Append("\"", game.Solution, "\"");
            _response.Append(" ", Messages.IsCorrectTheGameHasBeenSolved, " ", _partyEmote, " ");
            AppendWordStatus(game);
            _response.Append(", ");
            AppendWrongCharStatus(game);
            _response.Append(", ");
            AppendGuessStatus(game);
            _twitchBot.HangmanGames.Remove(ChatMessage.ChannelId);
            game.Dispose();
        }
        else if (game.WrongGuesses < HangmanGame.MaxWrongGuesses)
        {
            _response.Append("\"", guess, "\" ", Messages.IsNotCorrect, " ", _sadEmote);
            AppendWordStatus(game);
            _response.Append(", ");
            AppendWrongCharStatus(game);
            _response.Append(", ");
            AppendGuessStatus(game);
        }
        else
        {
            _response.Append(Messages.TheMaximumWrongGuessesHaveBeenReachedTheSolutionWas, " ", "\"", game.Solution, "\"");
            _response.Append(". ", _sadEmote);
            _twitchBot.HangmanGames.Remove(ChatMessage.ChannelId);
            game.Dispose();
        }
    }

    private void GuessChar()
    {
        if (!_twitchBot.HangmanGames.TryGetValue(ChatMessage.ChannelId, out HangmanGame? game))
        {
            _response.Append(ChatMessage.Username, ", ", Messages.ThereIsNoGameRunningYouHaveToStartOneFirst);
            return;
        }

        char guess = ChatMessage.Message[_alias.Length + (_prefix.Length == 0 ? AppSettings.Suffix.Length : _prefix.Length) + 1];
        guess = char.ToLowerInvariant(guess);
        int correctPlacesCount = game.Guess(guess);
        _response.Append(ChatMessage.Username, ", ");
        if (game.IsSolved)
        {
            _response.Append(Messages.TheGameHasBeenSolved, " ", _partyEmote, " ");
            _twitchBot.HangmanGames.Remove(ChatMessage.ChannelId);
            game.Dispose();
        }
        else if (game.WrongGuesses < HangmanGame.MaxWrongGuesses)
        {
            _response.Append('\'', guess, '\'', ' ');
            Span<char> buffer = stackalloc char[10];
            correctPlacesCount.TryFormat(buffer, out int bufferLength);
            _response.Append(Messages.IsCorrectIn, " ", buffer[..bufferLength]);
            _response.Append(" ", correctPlacesCount == 1 ? Messages.Places[..^1] : Messages.Places);
            _response.Append(" ", correctPlacesCount > 0 ? _happyEmote : _sadEmote, " ");
        }
        else
        {
            _response.Append(Messages.TheMaximumWrongGuessesHaveBeenReachedTheSolutionWas, " ", "\"", game.Solution, "\"");
            _response.Append(" ", _sadEmote, " ");
            _twitchBot.HangmanGames.Remove(ChatMessage.ChannelId);
            game.Dispose();
            return;
        }

        AppendWordStatus(game);
        _response.Append(", ");
        AppendWrongCharStatus(game);
        _response.Append(", ");
        AppendGuessStatus(game);
    }

    private void AppendWordStatus(HangmanGame game)
    {
        Span<char> buffer = stackalloc char[100];
        int bufferLength = StringHelper.Join(game.DiscoveredWord, ' ', buffer);
        _response.Append(buffer[..bufferLength]);
        game.Solution.Length.TryFormat(buffer, out bufferLength);
        _response.Append(" (", buffer[..bufferLength], " chars)");
    }

    private void AppendWrongCharStatus(HangmanGame game)
    {
        _response.Append(Messages.WrongChars, ":", " ");
        _response.Append('[');
        _response.Append(game.WrongChars);
        _response.Append(']');
    }

    private void AppendGuessStatus(HangmanGame game)
    {
        _response.Append(game.WrongGuesses);
        _response.Append('/');
        _response.Append(HangmanGame.MaxWrongGuesses);
        _response.Append(" ", Messages.WrongGuesses);
    }
}
