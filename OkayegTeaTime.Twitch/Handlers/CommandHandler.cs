using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using OkayegTeaTime.Database.Cache.Enums;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Controller;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Handlers;

public sealed class CommandHandler : Handler
{
    private readonly CommandExecutor _commandExecutor = new();
    private readonly AfkCommandHandler _afkCommandHandler;
    private readonly CooldownController _cooldownController;

    private readonly CommandType[] _commandTypes;
    private readonly AfkType[] _afkTypes = Enum.GetValues<AfkType>();

    public CommandHandler(TwitchBot twitchBot) : base(twitchBot)
    {
        _afkCommandHandler = new(twitchBot);
        _cooldownController = new(_twitchBot.CommandController);
        _commandTypes = GetHandledCommandTypes();
    }

    public override void Handle(TwitchChatMessage chatMessage)
    {
        bool handled = HandleCommand(chatMessage);
        if (!handled)
        {
            HandleAfkCommand(chatMessage);
        }
    }

    private bool HandleCommand(TwitchChatMessage chatMessage)
    {
        string? prefix = _twitchBot.Channels[chatMessage.ChannelId]?.Prefix;
        Span<CommandType> commandTypes = _commandTypes;
        int commandTypesLength = _commandTypes.Length;
        for (int i = 0; i < commandTypesLength; i++)
        {
            CommandType type = commandTypes[i];
            Span<string> aliases = _twitchBot.CommandController[type].Aliases;
            int aliasesLength = aliases.Length;
            for (int j = 0; j < aliasesLength; j++)
            {
                string alias = aliases[j];
                Regex pattern = _twitchBot.RegexCreator.Create(alias, prefix);
                if (!pattern.IsMatch(chatMessage.Message))
                {
                    continue;
                }

                if (_cooldownController.IsOnCooldown(chatMessage.UserId, type))
                {
                    return false;
                }

                _commandExecutor.Execute(type, _twitchBot, chatMessage, prefix, alias);
                _twitchBot.CommandCount++;
                _cooldownController.AddCooldown(chatMessage.UserId, type);
                return true;
            }
        }

        return false;
    }

    private void HandleAfkCommand(TwitchChatMessage chatMessage)
    {
        string? prefix = _twitchBot.Channels[chatMessage.ChannelId]?.Prefix;
        Span<AfkType> afkTypes = _afkTypes;
        int afkTypesLength = _afkTypes.Length;
        for (int i = 0; i < afkTypesLength; i++)
        {
            AfkType type = afkTypes[i];
            Span<string> aliases = _twitchBot.CommandController[type].Aliases;
            int aliasesLength = aliases.Length;
            for (int j = 0; j < aliasesLength; j++)
            {
                string alias = aliases[j];
                Regex pattern = _twitchBot.RegexCreator.Create(alias, prefix);
                if (!pattern.IsMatch(chatMessage.Message))
                {
                    continue;
                }

                if (_cooldownController.IsOnAfkCooldown(chatMessage.UserId))
                {
                    return;
                }

                _twitchBot.CommandCount++;
                _afkCommandHandler.Handle(chatMessage, type);
                _cooldownController.AddAfkCooldown(chatMessage.UserId);
                return;
            }
        }
    }

    private static CommandType[] GetHandledCommandTypes()
    {
        Span<Type> commands = Assembly.GetCallingAssembly().GetTypes().Where(t => t.GetCustomAttribute<HandledCommandAttribute>() is not null).ToArray();
        CommandType[] commandTypes = new CommandType[commands.Length];
        for (int i = 0; i < commandTypes.Length; i++)
        {
            commandTypes[i] = commands[i].GetCustomAttribute<HandledCommandAttribute>()!.CommandType;
        }

        return commandTypes.ToArray();
    }
}
