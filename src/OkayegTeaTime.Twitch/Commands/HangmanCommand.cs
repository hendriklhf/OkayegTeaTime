using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE;
using HLE.Collections;
using HLE.Text;
using HLE.Twitch.Tmi.Models;
using OkayegTeaTime.Resources;
using OkayegTeaTime.Configuration;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand<HangmanCommand>(CommandType.Hangman)]
public struct HangmanCommand(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    : IChatCommand<HangmanCommand>
{
    public PooledStringBuilder Response { get; } = new(GlobalSettings.MaxMessageLength);

    public ChatMessage ChatMessage { get; } = chatMessage;

    private readonly TwitchBot _twitchBot = twitchBot;
    private readonly ReadOnlyMemory<char> _prefix = prefix;
    private readonly ReadOnlyMemory<char> _alias = alias;

    private string? _happyEmote;
    private string? _sadEmote;
    private string? _partyEmote;

    private static readonly string[] s_hangmanWords = ResourceController.HangmanWords.Split(',');

    public static void Create(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out HangmanCommand command)
        => command = new(twitchBot, chatMessage, prefix, alias);

    public async ValueTask HandleAsync()
    {
        _happyEmote = await _twitchBot.EmoteService.GetBestEmoteAsync(ChatMessage.ChannelId, Emoji.Grinning, "happy", "good");
        _sadEmote = await _twitchBot.EmoteService.GetBestEmoteAsync(ChatMessage.ChannelId, Emoji.Cry, "cry", "sad", "bad");
        _partyEmote = await _twitchBot.EmoteService.GetBestEmoteAsync(ChatMessage.ChannelId, Emoji.PartyingFace, "cheer", "happy", "good");

        Regex pattern = _twitchBot.MessageRegexCreator.Create(_alias.Span, _prefix.Span, @"\s[a-z]");
        if (pattern.IsMatch(ChatMessage.Message.AsSpan()))
        {
            GuessChar();
            return;
        }

        pattern = _twitchBot.MessageRegexCreator.Create(_alias.Span, _prefix.Span, @"\s[a-z]{2,}");
        if (pattern.IsMatch(ChatMessage.Message.AsSpan()))
        {
            GuessWord();
            return;
        }

        pattern = _twitchBot.MessageRegexCreator.Create(_alias.Span, _prefix.Span);
        if (pattern.IsMatch(ChatMessage.Message.AsSpan()))
        {
            StartNewGame();
        }
    }

    private readonly void StartNewGame()
    {
        Response.Append($"{ChatMessage.Username}, ");
        if (_twitchBot.HangmanGames.ContainsKey(ChatMessage.ChannelId))
        {
            Response.Append(Texts.ThereIsAlreadyAGameRunning);
            return;
        }

        Debug.Assert(s_hangmanWords.Length != 0);
#pragma warning disable CA2000 // added to _twitchBot.HangmanGames, cant be disposed
        HangmanGame game = new(Random.Shared.GetItem(s_hangmanWords));
#pragma warning restore CA2000
        Response.Append($"{Texts.NewGameStarted}: ");
        AppendWordStatus(game);
        _twitchBot.HangmanGames.AddOrSet(ChatMessage.ChannelId, game);
    }

    private readonly void GuessWord()
    {
        if (!_twitchBot.HangmanGames.TryGetValue(ChatMessage.ChannelId, out HangmanGame? game))
        {
            Response.Append($"{ChatMessage.Username}, {Texts.ThereIsNoGameRunningYouHaveToStartOneFirst}");
            return;
        }

        using ChatMessageExtension messageExtension = new(ChatMessage);
        ReadOnlyMemory<char> guess = messageExtension.LowerSplit[1];
        bool isWordCorrect = game.Guess(guess.Span);
        Response.Append($"{ChatMessage.Username}");
        if (isWordCorrect)
        {
            Response.Append($"\"{game.Solution}\" {Texts.IsCorrectTheGameHasBeenSolved} {_partyEmote} ");
            AppendWordStatus(game);
            Response.Append(", ");
            AppendWrongCharStatus(game);
            Response.Append(", ");
            AppendGuessStatus(game);
            _twitchBot.HangmanGames.TryRemove(ChatMessage.ChannelId, out _);
            game.Dispose();
        }
        else if (game.WrongGuesses < HangmanGame.MaxWrongGuesses)
        {
            Response.Append($"\"{guess.Span}\" {Texts.IsNotCorrect} {_sadEmote} ");
            AppendWordStatus(game);
            Response.Append(", ");
            AppendWrongCharStatus(game);
            Response.Append(", ");
            AppendGuessStatus(game);
        }
        else
        {
            Response.Append($"{Texts.TheMaximumWrongGuessesHaveBeenReachedTheSolutionWas} \"{game.Solution}\"");
            _twitchBot.HangmanGames.TryRemove(ChatMessage.ChannelId, out _);
            game.Dispose();
        }
    }

    private readonly void GuessChar()
    {
        if (!_twitchBot.HangmanGames.TryGetValue(ChatMessage.ChannelId, out HangmanGame? game))
        {
            Response.Append($"{ChatMessage.Username}, {Texts.ThereIsNoGameRunningYouHaveToStartOneFirst}");
            return;
        }

        char guess = ChatMessage.Message[_alias.Length + (_prefix.Length == 0 ? GlobalSettings.Suffix.Length : _prefix.Length) + 1];
        guess = char.ToLowerInvariant(guess);
        int correctPlacesCount = game.Guess(guess);
        Response.Append($"{ChatMessage.Username}, ");
        if (game.IsSolved)
        {
            Response.Append($"{Texts.TheGameHasBeenSolved} {_partyEmote} ");
            _twitchBot.HangmanGames.TryRemove(ChatMessage.ChannelId, out _);
            game.Dispose();
        }
        else if (game.WrongGuesses < HangmanGame.MaxWrongGuesses)
        {
            Response.Append($"'{guess}' {Texts.IsCorrectIn} {correctPlacesCount} {(correctPlacesCount == 1 ? "place" : "places")} {(correctPlacesCount != 0 ? _happyEmote : _sadEmote)} ");
        }
        else
        {
            Response.Append($"{Texts.TheMaximumWrongGuessesHaveBeenReachedTheSolutionWas} \"{game.Solution}\" {_sadEmote}");
            _twitchBot.HangmanGames.TryRemove(ChatMessage.ChannelId, out _);
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
        int joinLength = StringHelpers.Join(' ', game.DiscoveredWord, Response.FreeBufferSpan);
        Response.Advance(joinLength);
        Response.Append($" ({game.Solution.Length} chars)");
    }

    private readonly void AppendWrongCharStatus(HangmanGame game) => Response.Append($"{Texts.WrongChars}: [{game.WrongChars}]");

    private readonly void AppendGuessStatus(HangmanGame game) => Response.Append($"{game.WrongGuesses} / {HangmanGame.MaxWrongGuesses} {Texts.WrongGuesses}");

    public readonly void Dispose() => Response.Dispose();

    public readonly bool Equals(HangmanCommand other) =>
        _twitchBot.Equals(other._twitchBot) && _prefix.Equals(other._prefix) && _alias.Equals(other._alias) &&
        Response.Equals(other.Response) && ChatMessage.Equals(other.ChatMessage);

    // ReSharper disable once ArrangeModifiersOrder
    public override readonly bool Equals(object? obj) => obj is HangmanCommand other && Equals(other);

    // ReSharper disable once ArrangeModifiersOrder
    public override readonly int GetHashCode() => HashCode.Combine(_twitchBot, _prefix, _alias, Response, ChatMessage);

    public static bool operator ==(HangmanCommand left, HangmanCommand right) => left.Equals(right);

    public static bool operator !=(HangmanCommand left, HangmanCommand right) => !left.Equals(right);
}
