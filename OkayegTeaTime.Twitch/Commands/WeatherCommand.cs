using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE.Strings;
using HLE.Twitch.Models;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Twitch.Models.OpenWeatherMap;
using OkayegTeaTime.Twitch.Services;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Weather, typeof(WeatherCommand))]
public readonly struct WeatherCommand(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    : IChatCommand<WeatherCommand>
{
    public PooledStringBuilder Response { get; } = new(GlobalSettings.MaxMessageLength);

    public IChatMessage ChatMessage { get; } = chatMessage;

    private readonly TwitchBot _twitchBot = twitchBot;
    private readonly ReadOnlyMemory<char> _prefix = prefix;
    private readonly ReadOnlyMemory<char> _alias = alias;

    public static void Create(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out WeatherCommand command)
        => command = new(twitchBot, chatMessage, prefix, alias);

    public async ValueTask HandleAsync()
    {
        string? city;
        bool isPrivateLocation;

        Regex pattern = _twitchBot.MessageRegexCreator.Create(_alias.Span, _prefix.Span, @"\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            using ChatMessageExtension messageExtension = new(ChatMessage);
            city = ChatMessage.Message[(messageExtension.Split[0].Length + 1)..];
            isPrivateLocation = false;
        }
        else
        {
            User? user = _twitchBot.Users[ChatMessage.UserId];
            city = user?.Location;
            if (user is null || city is null)
            {
                Response.Append(ChatMessage.Username, ", ", Messages.YouHaventSetYourLocationYet);
                return;
            }

            isPrivateLocation = user.IsPrivateLocation;
        }

        WeatherData? weatherData = await _twitchBot.WeatherService.GetWeatherAsync(city);
        if (weatherData is null)
        {
            Response.Append(ChatMessage.Username, ", ", Messages.ApiError);
            return;
        }

        if (weatherData.Message is not null)
        {
            Response.Append(ChatMessage.Username, ", ", weatherData.Message);
            return;
        }

        Response.Append(ChatMessage.Username, ", ");
        int charsWritten = WeatherService.FormatWeatherData(weatherData, Response.FreeBufferSpan, isPrivateLocation);
        Response.Advance(charsWritten);
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
