using System.Text.Json;
using System.Text.RegularExpressions;
using HLE.Emojis;
using HLE.Http;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Files;
using OkayegTeaTime.Files.Models;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

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

        OpenWeatherMapResponse? weatherData = GetWeatherData(city);
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

        Response = $"{ChatMessage.Username}, {(isPrivateLocation ? "(private location)" : $"{weatherData.CityName}, {weatherData.Location.Country}")}: " +
                   $"{weatherData.WeatherConditions[0].Description} {GetWeatherEmoji(weatherData.WeatherConditions[0].Id)}, {weatherData.Weather.Temperature}°C, " +
                   $"min. {weatherData.Weather.MinTemperature}°C, max. {weatherData.Weather.MaxTemperature}°C, {GetDirection(weatherData.Wind.Direction)} wind speed: {weatherData.Wind.Speed} m/s, " +
                   $"cloud cover: {weatherData.Clouds.Percentage}%, humidity: {weatherData.Weather.Humidity}%, air pressure: {weatherData.Weather.Pressure} hPa";
    }

    private static OpenWeatherMapResponse? GetWeatherData(string city)
    {
        HttpGet request = new($"https://api.openweathermap.org/data/2.5/weather?q={city}&units=metric&appid={AppSettings.OpenWeatherMapApiKey}");
        if (request.Result is null || !request.IsValidJsonData)
        {
            return null;
        }

        return JsonSerializer.Deserialize<OpenWeatherMapResponse>(request.Result);
    }

    private static string GetWeatherEmoji(int weatherId) =>
        weatherId switch
        {
            >= 200 and <= 231 => Emoji.CloudWithLightningAndRain,
            >= 300 and <= 321 => Emoji.CloudWithRain,
            >= 500 and <= 504 => Emoji.SunBehindRainCloud,
            511 => Emoji.CloudWithSnow,
            >= 520 and <= 531 => Emoji.CloudWithRain,
            >= 600 and <= 622 => Emoji.CloudWithSnow,
            762 => Emoji.Volcano,
            >= 701 and <= 771 => Emoji.Fog,
            781 => Emoji.Tornado,
            800 => Emoji.Sunny,
            801 => Emoji.SunBehindSmallCloud,
            802 => Emoji.SunBehindLargeCloud,
            803 or 804 => Emoji.Cloud,
            _ => Emoji.Question
        };

    private static string GetDirection(double deg) =>
        deg switch
        {
            < 11.25 => "N",
            < 33.75 => "NNE",
            < 56.25 => "NE",
            < 78.75 => "ENE",
            < 101.25 => "E",
            < 123.75 => "ESE",
            < 146.25 => "SE",
            < 168.75 => "SSE",
            < 191.25 => "S",
            < 213.75 => "SSW",
            < 236.25 => "SW",
            < 258.75 => "WSW",
            < 281.25 => "W",
            < 303.75 => "WNW",
            < 325.25 => "NW",
            < 348.75 => "NNW",
            <= 360 => "N",
            _ => Emoji.Question
        };
}
