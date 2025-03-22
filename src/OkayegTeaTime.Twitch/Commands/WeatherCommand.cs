using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE.Text;
using HLE.Twitch.Tmi.Models;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Configuration;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Twitch.Models.OpenWeatherMap;
using OkayegTeaTime.Twitch.Services;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand<WeatherCommand>(CommandType.Weather)]
public readonly struct WeatherCommand(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    : IChatCommand<WeatherCommand>
{
    public PooledStringBuilder Response { get; } = new(GlobalSettings.MaxMessageLength);

    public ChatMessage ChatMessage { get; } = chatMessage;

    private readonly TwitchBot _twitchBot = twitchBot;
    private readonly ReadOnlyMemory<char> _prefix = prefix;
    private readonly ReadOnlyMemory<char> _alias = alias;

    public static void Create(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out WeatherCommand command)
        => command = new(twitchBot, chatMessage, prefix, alias);

    public async ValueTask HandleAsync()
    {
        if (GlobalSettings.Settings.OpenWeatherMap is null)
        {
            Response.Append($"{ChatMessage.Username}, {Texts.TheCommandHasNotBeenConfiguredByTheBotOwner}");
            return;
        }

        string? city;
        bool isPrivateLocation;

        Regex pattern = _twitchBot.MessageRegexCreator.Create(_alias.Span, _prefix.Span, @"\s\S+");
        if (pattern.IsMatch(ChatMessage.Message.AsSpan()))
        {
            using ChatMessageExtension messageExtension = new(ChatMessage);
            city = new(ChatMessage.Message.AsSpan()[(messageExtension.Split[0].Length + 1)..]);
            isPrivateLocation = false;
        }
        else
        {
            User? user = _twitchBot.Users[ChatMessage.UserId];
            city = user?.Location;
            if (user is null || city is null)
            {
                Response.Append($"{ChatMessage.Username}, {Texts.YouHaventSetYourLocationYet}");
                return;
            }

            isPrivateLocation = user.IsPrivateLocation;
        }

        WeatherData? weatherData = await _twitchBot.WeatherService.GetWeatherAsync(city);
        if (weatherData is null)
        {
            Response.Append($"{ChatMessage.Username}, {Texts.ApiError}");
            return;
        }

        if (weatherData.Message is not null)
        {
            Response.Append($"{ChatMessage.Username}, {weatherData.Message}");

            return;
        }

        Response.Append(ChatMessage.Username);
        Response.Append(", ");
        WeatherService.FormatWeatherData(weatherData, Response, isPrivateLocation);
    }

    public void Dispose() => Response.Dispose();

    public bool Equals(WeatherCommand other) =>
        _twitchBot.Equals(other._twitchBot) && _prefix.Equals(other._prefix) && _alias.Equals(other._alias) &&
        Response.Equals(other.Response) && ChatMessage.Equals(other.ChatMessage);

    public override bool Equals(object? obj) => obj is WeatherCommand other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(_twitchBot, _prefix, _alias, Response, ChatMessage);

    public static bool operator ==(WeatherCommand left, WeatherCommand right) => left.Equals(right);

    public static bool operator !=(WeatherCommand left, WeatherCommand right) => !left.Equals(right);
}
