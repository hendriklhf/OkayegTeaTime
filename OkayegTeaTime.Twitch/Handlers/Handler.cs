﻿using System.Threading.Tasks;
using HLE.Twitch.Models;

namespace OkayegTeaTime.Twitch.Handlers;

public abstract class Handler(TwitchBot twitchBot)
{
    private protected readonly TwitchBot _twitchBot = twitchBot;

    // ReSharper disable once InconsistentNaming
    public abstract ValueTask HandleAsync(IChatMessage chatMessage);
}
