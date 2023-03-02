using System;
using System.Reflection;
using System.Runtime.InteropServices;
using HLE.Twitch;
using HLE.Twitch.Models;
using JetBrains.Annotations;
using OkayegTeaTime.Database;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Commands;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Handlers;

public sealed unsafe class CommandExecutor : IDisposable
{
    private readonly TwitchBot _twitchBot;
    private readonly delegate*<TwitchBot, ChatMessage, ref MessageBuilder, ReadOnlySpan<char>, ReadOnlySpan<char>, void>* _executionMethods;

    private const ushort _responseBufferSize = 2048;

    public CommandExecutor(TwitchBot twitchBot)
    {
        _twitchBot = twitchBot;

        Span<MethodInfo> methods = typeof(CommandExecutor).GetMethods(BindingFlags.NonPublic | BindingFlags.Static);
        nuint elementCount = (nuint)methods.Length;
        nuint elementSize = (nuint)sizeof(delegate*<TwitchBot, ChatMessage, ref MessageBuilder, ReadOnlySpan<char>, ReadOnlySpan<char>, void>);
        void* executionMethodsPointer = NativeMemory.Alloc(elementCount, elementSize);
        _executionMethods = (delegate*<TwitchBot, ChatMessage, ref MessageBuilder, ReadOnlySpan<char>, ReadOnlySpan<char>, void>*)executionMethodsPointer;

        for (int i = 0; i < methods.Length; i++)
        {
            nint functionPtr = methods[i].MethodHandle.GetFunctionPointer();
            _executionMethods[i] = (delegate*<TwitchBot, ChatMessage, ref MessageBuilder, ReadOnlySpan<char>, ReadOnlySpan<char>, void>)functionPtr;
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

    public void Execute(CommandType type, ChatMessage chatMessage, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        MessageBuilder response = new(_responseBufferSize);
        try
        {
            string emote = _twitchBot.Channels[chatMessage.Channel]?.Emote ?? AppSettings.DefaultEmote;
            response.Append(emote, " ");

            _executionMethods[(int)type](_twitchBot, chatMessage, ref response, prefix, alias);
            if (response.Length <= emote.Length + 1)
            {
                return;
            }

            _twitchBot.Send(chatMessage.ChannelId, response);
        }
        finally
        {
            response.Dispose();
        }
    }

    [UsedImplicitly]
    private static void ExecuteBanFromFileCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        try
        {
            BanFromFileCommand command = new(twitchBot, chatMessage, ref response, prefix, alias);
            command.Handle();
        }
        catch (Exception ex)
        {
            response.Clear();
            DbController.LogException(ex);
        }
    }

    [UsedImplicitly]
    private static void ExecuteChatterino7Command(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        try
        {
            Chatterino7Command command = new(twitchBot, chatMessage, ref response, prefix, alias);
            command.Handle();
        }
        catch (Exception ex)
        {
            response.Clear();
            DbController.LogException(ex);
        }
    }

    [UsedImplicitly]
    private static void ExecuteChatterinoCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        try
        {
            ChatterinoCommand command = new(twitchBot, chatMessage, ref response, prefix, alias);
            command.Handle();
        }
        catch (Exception ex)
        {
            response.Clear();
            DbController.LogException(ex);
        }
    }

    [UsedImplicitly]
    private static void ExecuteChattersCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        try
        {
            ChattersCommand command = new(twitchBot, chatMessage, ref response, prefix, alias);
            command.Handle();
        }
        catch (Exception ex)
        {
            response.Clear();
            DbController.LogException(ex);
        }
    }

    [UsedImplicitly]
    private static void ExecuteCheckCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        try
        {
            CheckCommand command = new(twitchBot, chatMessage, ref response, prefix, alias);
            command.Handle();
        }
        catch (Exception ex)
        {
            response.Clear();
            DbController.LogException(ex);
        }
    }

    [UsedImplicitly]
    private static void ExecuteCodeCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        try
        {
            CodeCommand command = new(twitchBot, chatMessage, ref response, prefix, alias);
            command.Handle();
        }
        catch (Exception ex)
        {
            response.Clear();
            DbController.LogException(ex);
        }
    }

    [UsedImplicitly]
    private static void ExecuteCoinFlipCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        try
        {
            CoinflipCommand command = new(twitchBot, chatMessage, ref response, prefix, alias);
            command.Handle();
        }
        catch (Exception ex)
        {
            response.Clear();
            DbController.LogException(ex);
        }
    }

    [UsedImplicitly]
    private static void ExecuteCSharpCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        try
        {
            CSharpCommand command = new(twitchBot, chatMessage, ref response, prefix, alias);
            command.Handle();
        }
        catch (Exception ex)
        {
            response.Clear();
            DbController.LogException(ex);
        }
    }

    [UsedImplicitly]
    private static void ExecuteFillCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        try
        {
            FillCommand command = new(twitchBot, chatMessage, ref response, prefix, alias);
            command.Handle();
        }
        catch (Exception ex)
        {
            response.Clear();
            DbController.LogException(ex);
        }
    }

    [UsedImplicitly]
    private static void ExecuteFormula1Command(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        try
        {
            Formula1Command command = new(twitchBot, chatMessage, ref response, prefix, alias);
            command.Handle();
        }
        catch (Exception ex)
        {
            response.Clear();
            DbController.LogException(ex);
        }
    }

    [UsedImplicitly]
    private static void ExecuteFuckCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        try
        {
            FuckCommand command = new(twitchBot, chatMessage, ref response, prefix, alias);
            command.Handle();
        }
        catch (Exception ex)
        {
            response.Clear();
            DbController.LogException(ex);
        }
    }

    [UsedImplicitly]
    private static void ExecuteGachiCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        try
        {
            GachiCommand command = new(twitchBot, chatMessage, ref response, prefix, alias);
            command.Handle();
        }
        catch (Exception ex)
        {
            response.Clear();
            DbController.LogException(ex);
        }
    }

    [UsedImplicitly]
    private static void ExecuteGuidCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        try
        {
            GuidCommand command = new(twitchBot, chatMessage, ref response, prefix, alias);
            command.Handle();
        }
        catch (Exception ex)
        {
            response.Clear();
            DbController.LogException(ex);
        }
    }

    [UsedImplicitly]
    private static void ExecuteHangmanCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        try
        {
            HangmanCommand command = new(twitchBot, chatMessage, ref response, prefix, alias);
            command.Handle();
        }
        catch (Exception ex)
        {
            response.Clear();
            DbController.LogException(ex);
        }
    }

    [UsedImplicitly]
    private static void ExecuteHelpCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        try
        {
            HelpCommand command = new(twitchBot, chatMessage, ref response, prefix, alias);
            command.Handle();
        }
        catch (Exception ex)
        {
            response.Clear();
            DbController.LogException(ex);
        }
    }

    [UsedImplicitly]
    private static void ExecuteIdCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        try
        {
            IdCommand command = new(twitchBot, chatMessage, ref response, prefix, alias);
            command.Handle();
        }
        catch (Exception ex)
        {
            response.Clear();
            DbController.LogException(ex);
        }
    }

    [UsedImplicitly]
    private static void ExecuteInvalidateCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        try
        {
            InvalidateCommand command = new(twitchBot, chatMessage, ref response, prefix, alias);
            command.Handle();
        }
        catch (Exception ex)
        {
            response.Clear();
            DbController.LogException(ex);
        }
    }

    [UsedImplicitly]
    private static void ExecuteJoinCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        try
        {
            JoinCommand command = new(twitchBot, chatMessage, ref response, prefix, alias);
            command.Handle();
        }
        catch (Exception ex)
        {
            response.Clear();
            DbController.LogException(ex);
        }
    }

    [UsedImplicitly]
    private static void ExecuteKotlinCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        try
        {
            KotlinCommand command = new(twitchBot, chatMessage, ref response, prefix, alias);
            command.Handle();
        }
        catch (Exception ex)
        {
            response.Clear();
            DbController.LogException(ex);
        }
    }

    [UsedImplicitly]
    private static void ExecuteLeaveCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        try
        {
            LeaveCommand command = new(twitchBot, chatMessage, ref response, prefix, alias);
            command.Handle();
        }
        catch (Exception ex)
        {
            response.Clear();
            DbController.LogException(ex);
        }
    }

    [UsedImplicitly]
    private static void ExecuteListenCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        try
        {
            ListenCommand command = new(twitchBot, chatMessage, ref response, prefix, alias);
            command.Handle();
        }
        catch (Exception ex)
        {
            response.Clear();
            DbController.LogException(ex);
        }
    }

    [UsedImplicitly]
    private static void ExecuteMasspingCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        try
        {
            MasspingCommand command = new(twitchBot, chatMessage, ref response, prefix, alias);
            command.Handle();
        }
        catch (Exception ex)
        {
            response.Clear();
            DbController.LogException(ex);
        }
    }

    [UsedImplicitly]
    private static void ExecuteMathCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        try
        {
            MathCommand command = new(twitchBot, chatMessage, ref response, prefix, alias);
            command.Handle();
        }
        catch (Exception ex)
        {
            response.Clear();
            DbController.LogException(ex);
        }
    }

    [UsedImplicitly]
    private static void ExecutePickCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        try
        {
            PickCommand command = new(twitchBot, chatMessage, ref response, prefix, alias);
            command.Handle();
        }
        catch (Exception ex)
        {
            response.Clear();
            DbController.LogException(ex);
        }
    }

    [UsedImplicitly]
    private static void ExecutePingCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        try
        {
            PingCommand command = new(twitchBot, chatMessage, ref response, prefix, alias);
            command.Handle();
        }
        catch (Exception ex)
        {
            response.Clear();
            DbController.LogException(ex);
        }
    }

    [UsedImplicitly]
    private static void ExecuteRafkCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        try
        {
            RafkCommand command = new(twitchBot, chatMessage, ref response, prefix, alias);
            command.Handle();
        }
        catch (Exception ex)
        {
            response.Clear();
            DbController.LogException(ex);
        }
    }

    [UsedImplicitly]
    private static void ExecuteRedditCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        try
        {
            RedditCommand command = new(twitchBot, chatMessage, ref response, prefix, alias);
            command.Handle();
        }
        catch (Exception ex)
        {
            response.Clear();
            DbController.LogException(ex);
        }
    }

    [UsedImplicitly]
    private static void ExecuteRemindCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        try
        {
            RemindCommand command = new(twitchBot, chatMessage, ref response, prefix, alias);
            command.Handle();
        }
        catch (Exception ex)
        {
            response.Clear();
            DbController.LogException(ex);
        }
    }

    [UsedImplicitly]
    private static void ExecuteSetCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        try
        {
            SetCommand command = new(twitchBot, chatMessage, ref response, prefix, alias);
            command.Handle();
        }
        catch (Exception ex)
        {
            response.Clear();
            DbController.LogException(ex);
        }
    }

    [UsedImplicitly]
    private static void ExecuteSkipCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        try
        {
            SkipCommand command = new(twitchBot, chatMessage, ref response, prefix, alias);
            command.Handle();
        }
        catch (Exception ex)
        {
            response.Clear();
            DbController.LogException(ex);
        }
    }

    [UsedImplicitly]
    private static void ExecuteSlotsCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        try
        {
            SlotsCommand command = new(twitchBot, chatMessage, ref response, prefix, alias);
            command.Handle();
        }
        catch (Exception ex)
        {
            response.Clear();
            DbController.LogException(ex);
        }
    }

    [UsedImplicitly]
    private static void ExecuteSongRequestCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        try
        {
            SongRequestCommand command = new(twitchBot, chatMessage, ref response, prefix, alias);
            command.Handle();
        }
        catch (Exception ex)
        {
            response.Clear();
            DbController.LogException(ex);
        }
    }

    [UsedImplicitly]
    private static void ExecuteSpotifyCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        try
        {
            SpotifyCommand command = new(twitchBot, chatMessage, ref response, prefix, alias);
            command.Handle();
        }
        catch (Exception ex)
        {
            response.Clear();
            DbController.LogException(ex);
        }
    }

    [UsedImplicitly]
    private static void ExecuteStreamCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        try
        {
            StreamCommand command = new(twitchBot, chatMessage, ref response, prefix, alias);
            command.Handle();
        }
        catch (Exception ex)
        {
            response.Clear();
            DbController.LogException(ex);
        }
    }

    [UsedImplicitly]
    private static void ExecuteSuggestCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        try
        {
            SuggestCommand command = new(twitchBot, chatMessage, ref response, prefix, alias);
            command.Handle();
        }
        catch (Exception ex)
        {
            response.Clear();
            DbController.LogException(ex);
        }
    }

    [UsedImplicitly]
    private static void ExecuteTuckCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        try
        {
            TuckCommand command = new(twitchBot, chatMessage, ref response, prefix, alias);
            command.Handle();
        }
        catch (Exception ex)
        {
            response.Clear();
            DbController.LogException(ex);
        }
    }

    [UsedImplicitly]
    private static void ExecuteUnsetCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        try
        {
            UnsetCommand command = new(twitchBot, chatMessage, ref response, prefix, alias);
            command.Handle();
        }
        catch (Exception ex)
        {
            response.Clear();
            DbController.LogException(ex);
        }
    }

    [UsedImplicitly]
    private static void ExecuteVanishCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        try
        {
            VanishCommand command = new(twitchBot, chatMessage, ref response, prefix, alias);
            command.Handle();
        }
        catch (Exception ex)
        {
            response.Clear();
            DbController.LogException(ex);
        }
    }

    [UsedImplicitly]
    private static void ExecuteWeatherCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        try
        {
            WeatherCommand command = new(twitchBot, chatMessage, ref response, prefix, alias);
            command.Handle();
        }
        catch (Exception ex)
        {
            response.Clear();
            DbController.LogException(ex);
        }
    }
}
