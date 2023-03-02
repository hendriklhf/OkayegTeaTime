using System;
using System.Text.RegularExpressions;
using HLE.Twitch;
using HLE.Twitch.Models;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Models.OpenWeatherMap;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Controller;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Weather)]
public readonly ref struct WeatherCommand
{
    public ChatMessage ChatMessage { get; }

    private readonly ref MessageBuilder _response;

    private readonly TwitchBot _twitchBot;
    private readonly ReadOnlySpan<char> _prefix;
    private readonly ReadOnlySpan<char> _alias;

    public WeatherCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        ChatMessage = chatMessage;
        _response = ref response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        string? city;
        bool isPrivateLocation;

        Regex pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\s\S+");
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
            isPrivateLocation = user?.IsPrivateLocation == true;
            if (city is null)
            {
                _response.Append(ChatMessage.Username, ", ", Messages.YouHaventSetYourLocationYet);
                return;
            }
        }

        WeatherData? weatherData = _twitchBot.WeatherController.GetWeather(city);
        if (weatherData is null)
        {
            _response.Append(ChatMessage.Username, ", ", Messages.ApiError);
            return;
        }

        if (weatherData.Message is not null)
        {
            _response.Append(ChatMessage.Username, ", ", weatherData.Message);
            return;
        }

        _response.Append(ChatMessage.Username, ", ", WeatherController.CreateResponse(weatherData, isPrivateLocation));
    }
}
