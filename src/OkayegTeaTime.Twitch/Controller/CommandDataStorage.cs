using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using OkayegTeaTime.Database.Cache.Enums;
using OkayegTeaTime.Configuration;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Controller;

[SuppressMessage("Performance", "CA1819:Properties should not return arrays")]
[SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
internal static class CommandDataStorage
{
    private const string ParameterNameNone = "(none)";
    private const string ParameterNameUsername = "[username]";
    private const string ParameterNameMessage = "[message]";

    public static ImmutableArray<Command> Commands { get; } =
    [
        new()
        {
            Type = CommandType.Chatterino,
            Name = "Chatterino",
            Description = "Sends useful Chatterino links, e.g. for download",
            Aliases = ["chatterino", "c2"],
            Parameters =
            [
                new(ParameterNameNone, "Sends useful Chatterino links, e.g. for download")
            ],
            Cooldown = TimeSpan.FromSeconds(10),
            Document = true
        },
        new()
        {
            Type = CommandType.Chatterino7,
            Name = "Chatterino7",
            Description = "Sends useful Chatterino7 links, e.g. for download",
            Aliases = ["chatterino7", "c7"],
            Parameters =
            [
                new(ParameterNameNone, "Sends useful Chatterino7 links, e.g. for download")
            ],
            Cooldown = TimeSpan.FromSeconds(10),
            Document = true
        },
        new()
        {
            Type = CommandType.Check,
            Name = "Check",
            Description = "Check afk states or reminders",
            Aliases = ["check"],
            Parameters =
            [
                new("afk [username]", "Checks the afk status of the given user"),
                new("reminder [id]", "Sends information about the reminder. Must be a running reminder")
            ],
            Cooldown = TimeSpan.FromSeconds(10),
            Document = true
        },
        new()
        {
            Type = CommandType.Code,
            Name = "Code",
            Description = "Sends links to source code of files matching the provided pattern",
            Aliases = ["code"],
            Parameters =
            [
                new("[regex file pattern]", "Sends links to source code of files matching the provided pattern")
            ],
            Cooldown = TimeSpan.FromSeconds(10),
            Document = true
        },
        new()
        {
            Type = CommandType.Coinflip,
            Name = "Coinflip",
            Description = "Flips a coin",
            Aliases = ["coinflip", "cf", "coin"],
            Parameters =
            [
                new(ParameterNameNone, "Flips a coin")
            ],
            Cooldown = TimeSpan.FromSeconds(5),
            Document = true
        },
        new()
        {
            Type = CommandType.CSharp,
            Name = "CSharp",
            Description = "Executes the given C# code",
            Aliases = ["csharp", "c#"],
            Parameters =
            [
                new("[code]", "Executes the given C# code")
            ],
            Cooldown = TimeSpan.FromSeconds(15),
            Document = true
        },
        new()
        {
            Type = CommandType.Fill,
            Name = "Fill",
            Description = "Sends a message that only contains elements of the given arguments up to its maximum length",
            Aliases = ["fill"],
            Parameters =
            [
                new("[arguments]", "Sends a message that only contains elements of the given arguments up to its maximum length")
            ],
            Cooldown = TimeSpan.FromSeconds(30),
            Document = true
        },
        new()
        {
            Type = CommandType.Formula1,
            Name = "Formula1",
            Description = "Sends information about the next Formula 1 event",
            Aliases = ["formula1", "f1"],
            Parameters =
            [
                new(ParameterNameNone, "Sends information about the next Formula 1 event"),
                new("weather", "Sends weather data for the Formula 1 location")
            ],
            Cooldown = TimeSpan.FromSeconds(10),
            Document = true
        },
        new()
        {
            Type = CommandType.Fuck,
            Name = "Fuck",
            Description = "Fucks the given user",
            Aliases = ["fuck"],
            Parameters =
            [
                new(ParameterNameUsername, "Fucks the given user"),
                new("[username] [emote]", "Fucks the given user and adds the emote")
            ],
            Cooldown = TimeSpan.FromSeconds(10),
            Document = true
        },
        new()
        {
            Type = CommandType.Gachi,
            Name = "Gachi",
            Description = "Sends a random gachi song",
            Aliases = ["gachi"],
            Parameters =
            [
                new(ParameterNameNone, "Sends a random gachi song")
            ],
            Cooldown = TimeSpan.FromSeconds(10),
            Document = true
        },
        new()
        {
            Type = CommandType.Guid,
            Name = "Guid",
            Description = "Sends a GUID",
            Aliases = ["guid", "uuid", "globallyuniqueidentifier", "universaluniqueidentifier"],
            Parameters =
            [
                new(ParameterNameNone, "Sends a GUID")
            ],
            Cooldown = TimeSpan.FromSeconds(5),
            Document = true
        },
        new()
        {
            Type = CommandType.Hangman,
            Name = "Hangman",
            Description = "Play a hangman game",
            Aliases = ["hangman", "hm"],
            Parameters =
            [
                new(ParameterNameNone, "Starts a new game"),
                new("[char]", "Guesses a char"),
                new("[word]", "Guesses a word")
            ],
            Cooldown = TimeSpan.FromSeconds(5),
            Document = true
        },
        new()
        {
            Type = CommandType.Help,
            Name = "Help",
            Description = "Sends a link to the GitHub repository",
            Aliases = ["help", "commands"],
            Parameters =
            [
                new(ParameterNameNone, "Sends a link to the GitHub repository")
            ],
            Cooldown = TimeSpan.FromSeconds(5),
            Document = true
        },
        new()
        {
            Type = CommandType.Id,
            Name = "Id",
            Description = "Sends Twitch user ids",
            Aliases = ["id", "uid", "userid", "channelid"],
            Parameters =
            [
                new(ParameterNameNone, "Sends your Twitch user id"),
                new(ParameterNameUsername, "Sends the Twitch user id of the given user")
            ],
            Cooldown = TimeSpan.FromSeconds(10),
            Document = true
        },
        new()
        {
            Type = CommandType.Invalidate,
            Name = "Invalidate",
            Description = "Invalidates all database caches",
            Aliases = ["invalidate"],
            Parameters =
            [
                new(ParameterNameNone, "Invalidates all database caches")
            ],
            Cooldown = TimeSpan.FromSeconds(10),
            Document = false
        },
        new()
        {
            Type = CommandType.Join,
            Name = "Join",
            Description = "Joins the given channel",
            Aliases = ["join", "enter"],
            Parameters =
            [
                new("[channel]", "Joins the given channel")
            ],
            Cooldown = TimeSpan.FromSeconds(10),
            Document = false
        },
        new()
        {
            Type = CommandType.Leave,
            Name = "Leave",
            Description = "Leaves the given channel",
            Aliases = ["leave", "part"],
            Parameters =
            [
                new("[channel]", "Leaves the given channel")
            ],
            Cooldown = TimeSpan.FromSeconds(10),
            Document = false
        },
        new()
        {
            Type = CommandType.Listen,
            Name = "Listen",
            Description = "Lets you listen along with another user on Spotify",
            Aliases = ["listen"],
            Parameters =
            [
                new(ParameterNameUsername, "Lets you listen along with another user")
            ],
            Cooldown = TimeSpan.FromSeconds(30),
            Document = false
        },
        new()
        {
            Type = CommandType.Massping,
            Name = "Massping",
            Description = "Pings all users in the current channel",
            Aliases = ["massping"],
            Parameters =
            [
                new(ParameterNameNone, "Pings all users in the current channel"),
                new("[emote]", "Pings all users in the current channel and adds the custom emote between each name")
            ],
            Cooldown = TimeSpan.FromSeconds(30),
            Document = false
        },
        new()
        {
            Type = CommandType.Math,
            Name = "Math",
            Description = "Calculates the solution of the given mathematical expression",
            Aliases = ["math", "calc"],
            Parameters =
            [
                new("[mathematical expression]", "Sends the solution of the given mathematical expression")
            ],
            Cooldown = TimeSpan.FromSeconds(10),
            Document = true
        },
        new()
        {
            Type = CommandType.Pick,
            Name = "Pick",
            Description = "Sends one item picked from all provided ones",
            Aliases = ["pick", "choose"],
            Parameters =
            [
                new("[options to pick from]", "Sends one item picked from all provided ones")
            ],
            Cooldown = TimeSpan.FromSeconds(5),
            Document = true
        },
        new()
        {
            Type = CommandType.Ping,
            Name = "Ping",
            Description = "Sends a ping message, if the bot is online",
            Aliases = ["ping", "pong"],
            Parameters =
            [
                new(ParameterNameNone, "Sends a ping message, if the bot is online")
            ],
            Cooldown = default,
            Document = false
        },
        new()
        {
            Type = CommandType.Rafk,
            Name = "Rafk",
            Description = "Resumes your last afk status",
            Aliases = ["rafk", "cafk"],
            Parameters =
            [
                new(ParameterNameNone, "Resumes your last afk status")
            ],
            Cooldown = TimeSpan.FromSeconds(5),
            Document = true
        },
        new()
        {
            Type = CommandType.Remind,
            Name = "Remind",
            Description = "Sets timed or non-timed for given users",
            Aliases = ["remind", "notify"],
            Parameters =
            [
                new("[username] [text]", "Sets a reminder for the given user"),
                new("[username] in [time] [text]", "Sets a reminder for the given user, that will be triggered after the given time")
            ],
            Cooldown = TimeSpan.FromSeconds(10),
            Document = true
        },
        new()
        {
            Type = CommandType.Set,
            Name = "Set",
            Description = "Sets things like channel prefix or emote",
            Aliases = ["set"],
            Parameters =
            [
                new("prefix [prefix]", $"Sets the channel's command prefix to the given prefix. A prefix can be as long as {GlobalSettings.MaxPrefixLength} chars"),
                new("emote [emote]", $"Sets the emote in front message to the given one. An emote can be as long as {GlobalSettings.MaxEmoteInFrontLength} chars"),
                new("location [private|public] [location]", "Sets your location for the weather command")
            ],
            Cooldown = TimeSpan.FromSeconds(5),
            Document = true
        },
        new()
        {
            Type = CommandType.Skip,
            Name = "Skip",
            Description = "Skips to the next song in the broadcaster's queue",
            Aliases = ["skip"],
            Parameters =
            [
                new(ParameterNameNone, "Skips to the next song in the broadcaster's queue, if registered and enabled")
            ],
            Cooldown = TimeSpan.FromSeconds(5),
            Document = true
        },
        new()
        {
            Type = CommandType.Slots,
            Name = "Slots",
            Description = "Rolls slots and picks 3 of all emotes",
            Aliases = ["slots"],
            Parameters =
            [
                new(ParameterNameNone, "Rolls slots and picks 3 of all emotes"),
                new("[regex]", "Rolls slots and picks 3 of all emotes matching the provided regex")
            ],
            Cooldown = TimeSpan.FromSeconds(5),
            Document = true
        },
        new()
        {
            Type = CommandType.SongRequest,
            Name = "SongRequest",
            Description = "Adds songs to Spotify queues",
            Aliases = ["songrequest", "sr"],
            Parameters =
            [
                new("[track]", "Adds the Spotify song to the broadcaster's queue, if registered and enabled"),
                new("[target] [source]", "Adds the source's Spotify song to the target's queue, if registered and enabled. Use \"me\" for your username")
            ],
            Cooldown = TimeSpan.FromSeconds(15),
            Document = true
        },
        new()
        {
            Type = CommandType.Spotify,
            Name = "Spotify",
            Description = "Sends songs users are listening to on Spotify",
            Aliases = ["spotify", "song"],
            Parameters =
            [
                new(ParameterNameNone, "Sends the current playing spotify song of the broadcaster, if registered"),
                new("me", "Sends your current playing spotify song, if registered"),
                new(ParameterNameUsername, "Sends the current playing spotify song of the given user, if registered")
            ],
            Cooldown = TimeSpan.FromSeconds(10),
            Document = true
        },
        new()
        {
            Type = CommandType.Stream,
            Name = "Stream",
            Description = "Sends information about a Twitch stream",
            Aliases = ["stream", "streaminfo", "si"],
            Parameters =
            [
                new(ParameterNameNone, "Sends information about the current channel's stream"),
                new("[channel]", "Sends information about the given channel's stream")
            ],
            Cooldown = TimeSpan.FromSeconds(10),
            Document = true
        },
        new()
        {
            Type = CommandType.Suggest,
            Name = "Suggest",
            Description = "Suggest or report anything concerning the bot",
            Aliases = ["suggest", "bug"],
            Parameters =
            [
                new(ParameterNameMessage, "Suggest or report anything concerning the bot with a provided message attached")
            ],
            Cooldown = TimeSpan.FromSeconds(10),
            Document = true
        },
        new()
        {
            Type = CommandType.Tuck,
            Name = "Tuck",
            Description = "Tucks the given user to bed",
            Aliases = ["tuck"],
            Parameters =
            [
                new(ParameterNameUsername, "Tucks the given user to bed"),
                new("[username] [emote]", "Tucks the given user to bed and appends the given emote")
            ],
            Cooldown = TimeSpan.FromSeconds(5),
            Document = true
        },
        new()
        {
            Type = CommandType.Unset,
            Name = "Unset",
            Description = "Unset things like channel prefix or emote",
            Aliases = ["unset"],
            Parameters =
            [
                new("prefix", "Unsets the command prefix in the current channel"),
                new("reminder [id]", "Unsets the reminder with the given id"),
                new("emote", "Unsets the emote in front in the current channel")
            ],
            Cooldown = TimeSpan.FromSeconds(10),
            Document = true
        },
        new()
        {
            Type = CommandType.Vanish,
            Name = "Vanish",
            Description = "Timeouts the sender for 1s",
            Aliases = ["vanish", "v"],
            Parameters =
            [
                new(ParameterNameNone, "Timeouts the sender for 1s")
            ],
            Cooldown = TimeSpan.FromSeconds(5),
            Document = true
        },
        new()
        {
            Type = CommandType.Weather,
            Name = "Weather",
            Description = "Sends weather data for the given location",
            Aliases = ["weather"],
            Parameters =
            [
                new("[location]", "Sends weather data for the given location")
            ],
            Cooldown = TimeSpan.FromSeconds(15),
            Document = true
        }
    ];

    public static ImmutableArray<AfkCommand> AfkCommands { get; } =
    [
        new()
        {
            Type = AfkType.Afk,
            Name = "Afk",
            ComingBack = "{} just came back: {} ({})",
            GoingAway = "{} is now afk",
            Resuming = "{} went afk again",
            Aliases = ["afk"],
            Parameters =
            [
                new(ParameterNameMessage, "Sets your status to \"afk\"")
            ],
            Document = true
        },
        new()
        {
            Type = AfkType.Brb,
            Name = "Brb",
            ComingBack = "{} came right back: {} ({})",
            GoingAway = "{} will be right back",
            Resuming = "{} went away again",
            Aliases = ["brb"],
            Parameters =
            [
                new(ParameterNameMessage, "Sets your status to \"brb\"")
            ],
            Document = true
        },
        new()
        {
            Type = AfkType.Fat,
            Name = "fat",
            ComingBack = "{} came back from eating: {} ({})",
            GoingAway = "{} went eating",
            Resuming = "{} went back to eating",
            Aliases = ["fat", "food"],
            Parameters =
            [
                new(ParameterNameMessage, "Sets your status to \"eating\"")
            ],
            Document = true
        },
        new()
        {
            Type = AfkType.Game,
            Name = "game",
            ComingBack = "{} stopped gaming: {} ({})",
            GoingAway = "{} is now gaming",
            Resuming = "{} went back to gaming",
            Aliases = ["game", "gaming"],
            Parameters =
            [
                new(ParameterNameMessage, "Sets your status to \"gaming\"")
            ],
            Document = true
        },
        new()
        {
            Type = AfkType.Gn,
            Name = "gn",
            ComingBack = "{} just woke up: {} ({})",
            GoingAway = "{} is now sleeping",
            Resuming = "{} fell asleep again",
            Aliases = ["gn", "sleep"],
            Parameters =
            [
                new(ParameterNameMessage, "Sets your status to \"sleeping\"")
            ],
            Document = true
        },
        new()
        {
            Type = AfkType.Lurk,
            Name = "lurk",
            ComingBack = "{} stopped lurking: {} ({})",
            GoingAway = "{} is now lurking",
            Resuming = "{} went back to lurking",
            Aliases = ["lurk"],
            Parameters =
            [
                new(ParameterNameMessage, "Sets your status to \"lurking\"")
            ],
            Document = true
        },
        new()
        {
            Type = AfkType.Movie,
            Name = "movie",
            ComingBack = "{} finished a movie: {} ({})",
            GoingAway = "{} is now watching a movie",
            Resuming = "{} went back to watching a movie",
            Aliases = ["movie", "film"],
            Parameters =
            [
                new(ParameterNameMessage, "Sets your status to \"watching a movie\"")
            ],
            Document = true
        },
        new()
        {
            Type = AfkType.Nap,
            Name = "nap",
            ComingBack = "{} woke up from their nap: {} ({})",
            GoingAway = "{} is taking a nap",
            Resuming = "{} went fell asleep again",
            Aliases = ["nap"],
            Parameters =
            [
                new(ParameterNameMessage, "Sets your status to \"napping\"")
            ],
            Document = true
        },
        new()
        {
            Type = AfkType.Piss,
            Name = "piss",
            ComingBack = "{} emptied their bladder: {} ({})",
            GoingAway = "{} needs to empty their bladder",
            Resuming = "{} ran back to the toilet",
            Aliases = ["piss", "pee"],
            Parameters =
            [
                new(ParameterNameMessage, "Sets your status to \"peeing\"")
            ],
            Document = true
        },
        new()
        {
            Type = AfkType.Poof,
            Name = "poof",
            ComingBack = "{} fooped back: {} ({})",
            GoingAway = "{} poofed away",
            Resuming = "{} poofed again ppPoof",
            Aliases = ["poof"],
            Parameters =
            [
                new(ParameterNameMessage, "Sets your status to \"poofed away\"")
            ],
            Document = true
        },
        new()
        {
            Type = AfkType.Poop,
            Name = "poop",
            ComingBack = "{} took a big shit: {} ({})",
            GoingAway = "{} needs to take a shit",
            Resuming = "{} ran back to the toilet",
            Aliases = ["poop", "shit"],
            Parameters =
            [
                new(ParameterNameMessage, "Sets your status to \"pooping\"")
            ],
            Document = true
        },
        new()
        {
            Type = AfkType.School,
            Name = "school",
            ComingBack = "{} came back from school: {} ({})",
            GoingAway = "{} went to school",
            Resuming = "{} went back to school",
            Aliases = ["school"],
            Parameters =
            [
                new(ParameterNameMessage, "Sets your status to \"at school\"")
            ],
            Document = true
        },
        new()
        {
            Type = AfkType.Shower,
            Name = "shower",
            ComingBack = "{} is now clean: {} ({})",
            GoingAway = "{} went for a shower",
            Resuming = "{} want back into the shower",
            Aliases = ["shower"],
            Parameters =
            [
                new(ParameterNameMessage, "Sets your status to \"showering\"")
            ],
            Document = true
        },
        new()
        {
            Type = AfkType.Study,
            Name = "study",
            ComingBack = "{} stopped studying: {} ({})",
            GoingAway = "{} is now studying",
            Resuming = "{} went back to studying",
            Aliases = ["study"],
            Parameters =
            [
                new(ParameterNameMessage, "Sets your status to \"studying\"")
            ],
            Document = true
        },
        new()
        {
            Type = AfkType.Work,
            Name = "work",
            ComingBack = "{} came back from work: {} ({})",
            GoingAway = "{} went to work",
            Resuming = "{} went back to work",
            Aliases = ["work"],
            Parameters =
            [
                new(ParameterNameMessage, "Sets your status to \"working\"")
            ],
            Document = true
        },
        new()
        {
            Type = AfkType.Music,
            Name = "music",
            ComingBack = "{} stopped listening to music: {} ({})",
            GoingAway = "{} is now listening to music",
            Resuming = "{} is listening to music again",
            Aliases = ["music"],
            Parameters =
            [
                new(ParameterNameMessage, "Sets your status to \"listening to music\"")
            ],
            Document = true
        },
        new()
        {
            Type = AfkType.Fap,
            Name = "fap",
            ComingBack = "{} came back from fapping: {} ({})",
            GoingAway = "{} went fapping",
            Resuming = "{} went back to fapping",
            Aliases = ["fap"],
            Parameters =
            [
                new(ParameterNameMessage, "Sets your status to \"fapping\"")
            ],
            Document = false
        },
        new()
        {
            Type = AfkType.Gym,
            Name = "gym",
            ComingBack = "{} stopped working out: {} ({})",
            GoingAway = "{} is now working out",
            Resuming = "{} is working out again",
            Aliases = ["gym", "workout"],
            Parameters =
            [
                new(ParameterNameMessage, "Sets your status to \"working out\"")
            ],
            Document = true
        }
    ];
}
