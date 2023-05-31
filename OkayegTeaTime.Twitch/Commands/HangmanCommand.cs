using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE.Collections;
using HLE.Emojis;
using HLE.Strings;
using HLE.Twitch.Models;
using OkayegTeaTime.Resources;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Hangman, typeof(HangmanCommand))]
public struct HangmanCommand : IChatCommand<HangmanCommand>
{
    public ResponseBuilder Response { get; }

    public ChatMessage ChatMessage { get; }

    private readonly TwitchBot _twitchBot;
    private readonly ReadOnlyMemory<char> _prefix;
    private readonly ReadOnlyMemory<char> _alias;

    private string? _happyEmote;
    private string? _sadEmote;
    private string? _partyEmote;

    private static readonly string[] _hangmanWords = ResourceController.HangmanWords.Split("\r\n");

    public HangmanCommand(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    {
        ChatMessage = chatMessage;
        Response = new(AppSettings.MaxMessageLength);
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public static void Create(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out HangmanCommand command)
    {
        command = new(twitchBot, chatMessage, prefix, alias);
    }

    public async ValueTask Handle()
    {
        _happyEmote = await _twitchBot.GetBestEmoteAsync(ChatMessage.ChannelId, Emoji.Grinning, "happy", "good");
        _sadEmote = await _twitchBot.GetBestEmoteAsync(ChatMessage.ChannelId, Emoji.Cry, "cry", "sad", "bad");
        _partyEmote = await _twitchBot.GetBestEmoteAsync(ChatMessage.ChannelId, Emoji.PartyingFace, "cheer", "happy", "good");

        Regex pattern = _twitchBot.RegexCreator.Create(_alias.Span, _prefix.Span, @"\s[a-z]");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            GuessChar();
            return;
        }

        pattern = _twitchBot.RegexCreator.Create(_alias.Span, _prefix.Span, @"\s[a-z]{2,}");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            GuessWord();
            return;
        }

        pattern = _twitchBot.RegexCreator.Create(_alias.Span, _prefix.Span);
        if (pattern.IsMatch(ChatMessage.Message))
        {
            StartNewGame();
        }
    }

    private readonly void StartNewGame()
    {
        Response.Append(ChatMessage.Username, ", ");
        if (_twitchBot.HangmanGames.TryGetValue(ChatMessage.ChannelId, out _))
        {
            Response.Append(Messages.ThereIsAlreadyAGameRunning);
            return;
        }

        Debug.Assert(_hangmanWords.Length > 0, "_hangmanWords.Length > 0");
        HangmanGame game = new(_hangmanWords.Random()!);
        Response.Append(Messages.NewGameStarted, ": ");
        AppendWordStatus(game);
        _twitchBot.HangmanGames.Add(ChatMessage.ChannelId, game);
    }

    private readonly void GuessWord()
    {
        if (!_twitchBot.HangmanGames.TryGetValue(ChatMessage.ChannelId, out HangmanGame? game))
        {
            Response.Append(ChatMessage.Username, ", ", Messages.ThereIsNoGameRunningYouHaveToStartOneFirst);
            return;
        }

        using ChatMessageExtension messageExtension = new(ChatMessage);
        ReadOnlyMemory<char> guess = messageExtension.LowerSplit[1];
        bool isWordCorrect = game.Guess(guess.Span);
        Response.Append(ChatMessage.Username, ", ");
        if (isWordCorrect)
        {
            Response.Append("\"", game.Solution, "\"");
            Response.Append(" ", Messages.IsCorrectTheGameHasBeenSolved, " ", _partyEmote, " ");
            AppendWordStatus(game);
            Response.Append(", ");
            AppendWrongCharStatus(game);
            Response.Append(", ");
            AppendGuessStatus(game);
            _twitchBot.HangmanGames.Remove(ChatMessage.ChannelId);
            game.Dispose();
        }
        else if (game.WrongGuesses < HangmanGame.MaxWrongGuesses)
        {
            Response.Append("\"", guess.Span, "\" ", Messages.IsNotCorrect, " ", _sadEmote);
            AppendWordStatus(game);
            Response.Append(", ");
            AppendWrongCharStatus(game);
            Response.Append(", ");
            AppendGuessStatus(game);
        }
        else
        {
            Response.Append(Messages.TheMaximumWrongGuessesHaveBeenReachedTheSolutionWas, " ", "\"", game.Solution, "\"");
            Response.Append(". ", _sadEmote);
            _twitchBot.HangmanGames.Remove(ChatMessage.ChannelId);
            game.Dispose();
        }
    }

    private readonly void GuessChar()
    {
        if (!_twitchBot.HangmanGames.TryGetValue(ChatMessage.ChannelId, out HangmanGame? game))
        {
            Response.Append(ChatMessage.Username, ", ", Messages.ThereIsNoGameRunningYouHaveToStartOneFirst);
            return;
        }

        char guess = ChatMessage.Message[_alias.Length + (_prefix.Length == 0 ? AppSettings.Suffix.Length : _prefix.Length) + 1];
        guess = char.ToLowerInvariant(guess);
        int correctPlacesCount = game.Guess(guess);
        Response.Append(ChatMessage.Username, ", ");
        if (game.IsSolved)
        {
            Response.Append(Messages.TheGameHasBeenSolved, " ", _partyEmote, " ");
            _twitchBot.HangmanGames.Remove(ChatMessage.ChannelId);
            game.Dispose();
        }
        else if (game.WrongGuesses < HangmanGame.MaxWrongGuesses)
        {
            Response.Append('\'', guess, '\'', ' ');
            Span<char> buffer = stackalloc char[10];
            correctPlacesCount.TryFormat(buffer, out int bufferLength);
            Response.Append(Messages.IsCorrectIn, " ", buffer[..bufferLength]);
            Response.Append(" ", correctPlacesCount == 1 ? "place" : "places");
            Response.Append(" ", correctPlacesCount > 0 ? _happyEmote : _sadEmote, " ");
        }
        else
        {
            Response.Append(Messages.TheMaximumWrongGuessesHaveBeenReachedTheSolutionWas, " ", "\"", game.Solution, "\"");
            Response.Append(" ", _sadEmote, " ");
            _twitchBot.HangmanGames.Remove(ChatMessage.ChannelId);
            game.Dispose();
            return;
        }

        AppendWordStatus(game);
        Response.Append(", ");
        AppendWrongCharStatus(game);
        Response.Append(", ");
        AppendGuessStatus(game);
    }

    private readonly void AppendWordStatus(HangmanGame game)
    {
        Span<char> buffer = stackalloc char[100];
        int bufferLength = StringHelper.Join(game.DiscoveredWord, ' ', buffer);
        Response.Append(buffer[..bufferLength]);
        game.Solution.Length.TryFormat(buffer, out bufferLength);
        Response.Append(" (", buffer[..bufferLength], " chars)");
    }

    private readonly void AppendWrongCharStatus(HangmanGame game)
    {
        Response.Append(Messages.WrongChars, ":", " ");
        Response.Append('[');
        Response.Append(game.WrongChars);
        Response.Append(']');
    }

    private readonly void AppendGuessStatus(HangmanGame game)
    {
        Response.Append(game.WrongGuesses);
        Response.Append('/');
        Response.Append(HangmanGame.MaxWrongGuesses);
        Response.Append(" ", Messages.WrongGuesses);
    }

    public readonly void Dispose()
    {
        Response.Dispose();
    }
}
