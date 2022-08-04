using System.Text.RegularExpressions;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Files.Models;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Weather)]
public class WeatherCommand : Command
{
    public WeatherCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        string? city;
        bool isPrivateLocation;

        Regex pattern = PatternCreator.Create(_alias, _prefix, @"\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            city = ChatMessage.Message[(ChatMessage.Split[0].Length + 1)..];
            isPrivateLocation = false;
        }
        else
        {
            User? user = DbControl.Users[ChatMessage.UserId];
            city = user?.Location;
            isPrivateLocation = user?.IsPrivateLocation == true;
            if (city is null)
            {
                Response = $"{ChatMessage.Username}, you haven't set your location yet";
                return;
            }
        }

        OwmWeatherData? weatherData = _twitchBot.WeatherController.GetWeather(city);
        if (weatherData is null)
        {
            Response = $"{ChatMessage.Username}, api error";
            return;
        }

        if (weatherData.Message is not null)
        {
            Response = $"{ChatMessage.Username}, {weatherData.Message}";
            return;
        }

        Response = $"{ChatMessage.Username}, {_twitchBot.WeatherController.CreateResponse(weatherData, isPrivateLocation)}";
    }
}
