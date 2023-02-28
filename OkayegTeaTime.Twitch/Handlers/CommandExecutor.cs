using System;
using System.Reflection;
using System.Runtime.InteropServices;
using HLE;
using HLE.Twitch.Models;
using JetBrains.Annotations;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Commands;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Handlers;

public sealed unsafe class CommandExecutor : IDisposable
{
    private readonly TwitchBot _twitchBot;
    private readonly delegate*<TwitchBot, ChatMessage, StringBuilder*, string?, string, void>* _executionMethods;

    private const ushort _responseBufferSize = 2048;

    public CommandExecutor(TwitchBot twitchBot)
    {
        _twitchBot = twitchBot;

        Span<MethodInfo> methods = typeof(CommandExecutor).GetMethods(BindingFlags.NonPublic | BindingFlags.Static);
        nuint elementCount = (nuint)methods.Length;
        nuint elementSize = (nuint)sizeof(delegate*<TwitchBot, ChatMessage, StringBuilder*, string?, string, void>);
        void* executionMethodsPointer = NativeMemory.Alloc(elementCount, elementSize);
        _executionMethods = (delegate*<TwitchBot, ChatMessage, StringBuilder*, string?, string, void>*)executionMethodsPointer;

        for (int i = 0; i < methods.Length; i++)
        {
            nint functionPtr = methods[i].MethodHandle.GetFunctionPointer();
            _executionMethods[i] = (delegate*<TwitchBot, ChatMessage, StringBuilder*, string?, string, void>)functionPtr;
        }
    }

    ~CommandExecutor()
    {
        NativeMemory.Free(_executionMethods);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        NativeMemory.Free(_executionMethods);
    }

    public void Execute(CommandType type, ChatMessage chatMessage, string? prefix, string alias)
    {
        StringBuilder response = stackalloc char[_responseBufferSize];
        string emote = _twitchBot.Channels[chatMessage.Channel]?.Emote ?? AppSettings.DefaultEmote;
        response.Append(emote, StringHelper.Whitespace);

        _executionMethods[(int)type](_twitchBot, chatMessage, &response, prefix!, alias);
        if (response.Length <= emote.Length + 1)
        {
            return;
        }

        if (response.Equals(_twitchBot.LastMessages[chatMessage.Channel], StringComparison.Ordinal))
        {
            response.Append(AppSettings.ChatterinoChar);
        }

        _twitchBot.Send(chatMessage.Channel, response.ToString(), false, true, false);
    }

    [UsedImplicitly]
    private static void ExecuteBanFromFileCommand(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        BanFromFileCommand command = new(twitchBot, chatMessage, response, prefix, alias);
        command.Handle();
    }

    [UsedImplicitly]
    private static void ExecuteChatterino7Command(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        Chatterino7Command command = new(twitchBot, chatMessage, response, prefix, alias);
        command.Handle();
    }

    [UsedImplicitly]
    private static void ExecuteChatterinoCommand(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        ChatterinoCommand command = new(twitchBot, chatMessage, response, prefix, alias);
        command.Handle();
    }

    [UsedImplicitly]
    private static void ExecuteChattersCommand(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        ChattersCommand command = new(twitchBot, chatMessage, response, prefix, alias);
        command.Handle();
    }

    [UsedImplicitly]
    private static void ExecuteCheckCommand(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        CheckCommand command = new(twitchBot, chatMessage, response, prefix, alias);
        command.Handle();
    }

    [UsedImplicitly]
    private static void ExecuteCodeCommand(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        CodeCommand command = new(twitchBot, chatMessage, response, prefix, alias);
        command.Handle();
    }

    [UsedImplicitly]
    private static void ExecuteCoinFlipCommand(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        CoinflipCommand command = new(twitchBot, chatMessage, response, prefix, alias);
        command.Handle();
    }

    [UsedImplicitly]
    private static void ExecuteCSharpCommand(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        CSharpCommand command = new(twitchBot, chatMessage, response, prefix, alias);
        command.Handle();
    }

    [UsedImplicitly]
    private static void ExecuteFillCommand(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        FillCommand command = new(twitchBot, chatMessage, response, prefix, alias);
        command.Handle();
    }

    [UsedImplicitly]
    private static void ExecuteFormula1Command(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        Formula1Command command = new(twitchBot, chatMessage, response, prefix, alias);
        command.Handle();
    }

    [UsedImplicitly]
    private static void ExecuteFuckCommand(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        FuckCommand command = new(twitchBot, chatMessage, response, prefix, alias);
        command.Handle();
    }

    [UsedImplicitly]
    private static void ExecuteGachiCommand(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        GachiCommand command = new(twitchBot, chatMessage, response, prefix, alias);
        command.Handle();
    }

    [UsedImplicitly]
    private static void ExecuteGuidCommand(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        GuidCommand command = new(twitchBot, chatMessage, response, prefix, alias);
        command.Handle();
    }

    [UsedImplicitly]
    private static void ExecuteHangmanCommand(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        HangmanCommand command = new(twitchBot, chatMessage, response, prefix, alias);
        command.Handle();
    }

    [UsedImplicitly]
    private static void ExecuteHelpCommand(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        HelpCommand command = new(twitchBot, chatMessage, response, prefix, alias);
        command.Handle();
    }

    [UsedImplicitly]
    private static void ExecuteIdCommand(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        IdCommand command = new(twitchBot, chatMessage, response, prefix, alias);
        command.Handle();
    }

    [UsedImplicitly]
    private static void ExecuteInvalidateCommand(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        InvalidateCommand command = new(twitchBot, chatMessage, response, prefix, alias);
        command.Handle();
    }

    [UsedImplicitly]
    private static void ExecuteJoinCommand(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        JoinCommand command = new(twitchBot, chatMessage, response, prefix, alias);
        command.Handle();
    }

    [UsedImplicitly]
    private static void ExecuteKotlinCommand(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        KotlinCommand command = new(twitchBot, chatMessage, response, prefix, alias);
        command.Handle();
    }

    [UsedImplicitly]
    private static void ExecuteLeaveCommand(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        LeaveCommand command = new(twitchBot, chatMessage, response, prefix, alias);
        command.Handle();
    }

    [UsedImplicitly]
    private static void ExecuteListenCommand(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        ListenCommand command = new(twitchBot, chatMessage, response, prefix, alias);
        command.Handle();
    }

    [UsedImplicitly]
    private static void ExecuteMasspingCommand(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        MasspingCommand command = new(twitchBot, chatMessage, response, prefix, alias);
        command.Handle();
    }

    [UsedImplicitly]
    private static void ExecuteMathCommand(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        MathCommand command = new(twitchBot, chatMessage, response, prefix, alias);
        command.Handle();
    }

    [UsedImplicitly]
    private static void ExecutePickCommand(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        PickCommand command = new(twitchBot, chatMessage, response, prefix, alias);
        command.Handle();
    }

    [UsedImplicitly]
    private static void ExecutePingCommand(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        PingCommand command = new(twitchBot, chatMessage, response, prefix, alias);
        command.Handle();
    }

    [UsedImplicitly]
    private static void ExecuteRafkCommand(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        RafkCommand command = new(twitchBot, chatMessage, response, prefix, alias);
        command.Handle();
    }

    [UsedImplicitly]
    private static void ExecuteRedditCommand(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        RedditCommand command = new(twitchBot, chatMessage, response, prefix, alias);
        command.Handle();
    }

    [UsedImplicitly]
    private static void ExecuteRemindCommand(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        RemindCommand command = new(twitchBot, chatMessage, response, prefix, alias);
        command.Handle();
    }

    [UsedImplicitly]
    private static void ExecuteSetCommand(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        SetCommand command = new(twitchBot, chatMessage, response, prefix, alias);
        command.Handle();
    }

    [UsedImplicitly]
    private static void ExecuteSkipCommand(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        SkipCommand command = new(twitchBot, chatMessage, response, prefix, alias);
        command.Handle();
    }

    [UsedImplicitly]
    private static void ExecuteSlotsCommand(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        SlotsCommand command = new(twitchBot, chatMessage, response, prefix, alias);
        command.Handle();
    }

    [UsedImplicitly]
    private static void ExecuteSongRequestCommand(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        SongRequestCommand command = new(twitchBot, chatMessage, response, prefix, alias);
        command.Handle();
    }

    [UsedImplicitly]
    private static void ExecuteSpotifyCommand(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        SpotifyCommand command = new(twitchBot, chatMessage, response, prefix, alias);
        command.Handle();
    }

    [UsedImplicitly]
    private static void ExecuteStreamCommand(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        StreamCommand command = new(twitchBot, chatMessage, response, prefix, alias);
        command.Handle();
    }

    [UsedImplicitly]
    private static void ExecuteSuggestCommand(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        SuggestCommand command = new(twitchBot, chatMessage, response, prefix, alias);
        command.Handle();
    }

    [UsedImplicitly]
    private static void ExecuteTuckCommand(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        TuckCommand command = new(twitchBot, chatMessage, response, prefix, alias);
        command.Handle();
    }

    [UsedImplicitly]
    private static void ExecuteUnsetCommand(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        UnsetCommand command = new(twitchBot, chatMessage, response, prefix, alias);
        command.Handle();
    }

    [UsedImplicitly]
    private static void ExecuteVanishCommand(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        VanishCommand command = new(twitchBot, chatMessage, response, prefix, alias);
        command.Handle();
    }

    [UsedImplicitly]
    private static void ExecuteWeatherCommand(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        WeatherCommand command = new(twitchBot, chatMessage, response, prefix, alias);
        command.Handle();
    }
}
