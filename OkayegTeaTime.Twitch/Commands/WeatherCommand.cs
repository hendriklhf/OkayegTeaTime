using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE.Twitch.Models;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Models.OpenWeatherMap;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Twitch.Services;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Weather, typeof(WeatherCommand))]
public readonly struct WeatherCommand : IChatCommand<WeatherCommand>
{
    public ResponseBuilder Response { get; }

    public ChatMessage ChatMessage { get; }

    private readonly TwitchBot _twitchBot;
    private readonly ReadOnlyMemory<char> _prefix;
    private readonly ReadOnlyMemory<char> _alias;

    private WeatherCommand(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    {
        ChatMessage = chatMessage;
        Response = new(AppSettings.MaxMessageLength);
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public static void Create(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out WeatherCommand command)
    {
        command = new(twitchBot, chatMessage, prefix, alias);
    }

    public async ValueTask Handle()
    {
        string? city;
        bool isPrivateLocation;

        Regex pattern = _twitchBot.RegexCreator.Create(_alias.Span, _prefix.Span, @"\s\S+");
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
        int charsWritten = WeatherService.WriteWeatherData(weatherData, Response.FreeBufferSpan, isPrivateLocation);
        Response.Advance(charsWritten);
    }

    public void Dispose()
    {
        Response.Dispose();
    }
}
