using System.Text.Json;
using System.Text.RegularExpressions;
using HLE.Emojis;
using HLE.Http;
using OkayegTeaTime.Files;
using OkayegTeaTime.Files.Jsons.HttpRequests.OpenWeatherMap;
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
        Regex pattern = PatternCreator.Create(Alias, Prefix, @"\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            string city = ChatMessage.Message[(ChatMessage.Split[0].Length + 1)..];
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

            Response = $"{ChatMessage.Username}, {weatherData.CityName}, {weatherData.Location.Country}: Temperature: {weatherData.Weather.Temperature}°C, " +
                       $"min. temperature: {weatherData.Weather.MinTemperature}°C, max. temperature: {weatherData.Weather.MaxTemperature}°C, " +
                       $"{GetDirection(weatherData.Wind.Direction)} wind speed: {weatherData.Wind.Speed} m/s, cloud cover: {weatherData.Clouds.Percentage}%, " +
                       $"humidity: {weatherData.Weather.Humidity}%, air pressure: {weatherData.Weather.Pressure} hPa";
        }
    }

    private OpenWeatherMapResponse? GetWeatherData(string city)
    {
        HttpGet request = new($"https://api.openweathermap.org/data/2.5/weather?q={city}&units=metric&appid={AppSettings.OpenWeatherMapApiKey}");
        if (request.Result is null || !request.IsValidJsonData)
        {
            return null;
        }

        return JsonSerializer.Deserialize<OpenWeatherMapResponse>(request.Result);
    }

    private string GetDirection(double deg) =>
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
