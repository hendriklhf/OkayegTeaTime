using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HLE.Twitch.Models;
using OkayegTeaTime.Database;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Commands;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Handlers;

public sealed class CommandExecutor
{
    private readonly TwitchBot _twitchBot;
    private readonly Func<TwitchBot, ChatMessage, ReadOnlyMemory<char>, ReadOnlyMemory<char>, ValueTask>[] _executionMethods;

    public CommandExecutor(TwitchBot twitchBot)
    {
        _twitchBot = twitchBot;

        MethodInfo executionMethod = typeof(CommandExecutor).GetMethod(nameof(ExecuteCommandAsync), BindingFlags.Static | BindingFlags.NonPublic)!;
        HandledCommandAttribute[] handledCommandAttributes = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.GetCustomAttribute<HandledCommandAttribute>() is not null).Select(t => t.GetCustomAttribute<HandledCommandAttribute>()!).ToArray();
        Dictionary<CommandType, Type> handledCommandTypes = handledCommandAttributes.ToDictionary(a => a.CommandType, a => a.Command);
        int methodCount = (int)handledCommandTypes.Keys.Max() + 1;
        _executionMethods = new Func<TwitchBot, ChatMessage, ReadOnlyMemory<char>, ReadOnlyMemory<char>, ValueTask>[methodCount];
        foreach (KeyValuePair<CommandType, Type> command in handledCommandTypes)
        {
            var executionFunction = executionMethod.MakeGenericMethod(command.Value).CreateDelegate<Func<TwitchBot, ChatMessage, ReadOnlyMemory<char>, ReadOnlyMemory<char>, ValueTask>>();
            _executionMethods[(int)command.Key] = executionFunction;
        }
    }

    public async ValueTask ExecuteAsync(CommandType type, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    {
        await _executionMethods[(int)type](_twitchBot, chatMessage, prefix, alias);
    }

    private static async ValueTask ExecuteCommandAsync<T>(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias) where T : IChatCommand<T>
    {
        T.Create(twitchBot, chatMessage, prefix, alias, out T command);
        try
        {
            string emote = twitchBot.Channels[chatMessage.Channel]?.Emote ?? AppSettings.DefaultEmote;
            command.Response.Append(emote);
            command.Response.Append(' ');
            int responseLengthBeforeHandle = command.Response.Length;

            await command.Handle();
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
