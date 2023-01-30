using System.Text.RegularExpressions;
using HLE;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Files.Models;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Controller;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Weather)]
public readonly unsafe ref struct WeatherCommand
{
    public TwitchChatMessage ChatMessage { get; }

    public StringBuilder* Response { get; }

    private readonly TwitchBot _twitchBot;
    private readonly string? _prefix;
    private readonly string _alias;

    public WeatherCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        ChatMessage = chatMessage;
        Response = response;
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
            city = ChatMessage.Message[(ChatMessage.Split[0].Length + 1)..];
            isPrivateLocation = false;
        }
        else
        {
            User? user = _twitchBot.Users[ChatMessage.UserId];
            city = user?.Location;
            isPrivateLocation = user?.IsPrivateLocation == true;
            if (city is null)
            {
                Response->Append(ChatMessage.Username, Messages.CommaSpace, Messages.YouHaventSetYourLocationYet);
                return;
            }
        }

        OwmWeatherData? weatherData = _twitchBot.WeatherController.GetWeather(city);
        if (weatherData is null)
        {
            Response->Append(ChatMessage.Username, Messages.CommaSpace, Messages.ApiError);
            return;
        }

        if (weatherData.Message is not null)
        {
            Response->Append(ChatMessage.Username, Messages.CommaSpace, weatherData.Message);
            return;
        }

        Response->Append(ChatMessage.Username, Messages.CommaSpace, WeatherController.CreateResponse(weatherData, isPrivateLocation));
    }
}
