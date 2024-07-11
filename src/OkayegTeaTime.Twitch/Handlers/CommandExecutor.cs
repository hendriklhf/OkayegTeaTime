using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using HLE.Twitch.Models;
using OkayegTeaTime.Database;
using OkayegTeaTime.Configuration;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Commands;
using OkayegTeaTime.Twitch.Models;
using ExecutionMethod = System.Func<OkayegTeaTime.Twitch.TwitchBot, HLE.Twitch.Models.IChatMessage, System.ReadOnlyMemory<char>, System.ReadOnlyMemory<char>, System.Threading.Tasks.ValueTask>;

namespace OkayegTeaTime.Twitch.Handlers;

public sealed class CommandExecutor
{
    private readonly TwitchBot _twitchBot;
    private readonly ExecutionMethod[] _executionMethods;

    [SuppressMessage("Major Code Smell", "S3011:Reflection should not be used to increase accessibility of classes, methods, or fields")]
    public CommandExecutor(TwitchBot twitchBot)
    {
        _twitchBot = twitchBot;

        MethodInfo executionMethod = typeof(CommandExecutor).GetMethod(nameof(ExecuteCommandAsync), BindingFlags.Static | BindingFlags.NonPublic)!;
        HandledCommandAttribute[] handledCommandAttributes = typeof(CommandExecutor).Assembly
            .GetTypes()
            .Where(static t => t.GetCustomAttribute<HandledCommandAttribute>() is not null)
            .Select(static t => t.GetCustomAttribute<HandledCommandAttribute>()!)
            .ToArray();

        int methodCount = (int)handledCommandAttributes.MaxBy(static a => a.CommandType)!.CommandType + 1;
        _executionMethods = new ExecutionMethod[methodCount];
        foreach (HandledCommandAttribute attribute in handledCommandAttributes)
        {
            ExecutionMethod executionFunction = executionMethod.MakeGenericMethod(attribute.Command).CreateDelegate<ExecutionMethod>();
            _executionMethods[(int)attribute.CommandType] = executionFunction;
        }
    }

    // ReSharper disable once InconsistentNaming
    public ValueTask ExecuteAsync(CommandType type, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    {
        ExecutionMethod executionMethod = Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(_executionMethods), (int)type);
        return executionMethod(_twitchBot, chatMessage, prefix, alias);
    }

    private static async ValueTask ExecuteCommandAsync<T>(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
        where T : IChatCommand<T>
    {
        T.Create(twitchBot, chatMessage, prefix, alias, out T command);
        try
        {
            string emote = twitchBot.Channels[chatMessage.Channel]?.Emote ?? GlobalSettings.DefaultEmote;
            command.Response.Append(emote);
            command.Response.Append(' ');
            int responseLengthBeforeHandle = command.Response.Length;

            await command.HandleAsync();
            if (command.Response.Length <= responseLengthBeforeHandle)
            {
                return;
            }

            await twitchBot.SendAsync(chatMessage.ChannelId, command.Response.WrittenMemory);
        }
        catch (Exception ex)
        {
            await DbController.LogExceptionAsync(ex);
        }
        finally
        {
            command.Dispose();
        }
    }
}
