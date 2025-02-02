using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using HLE.Collections.Concurrent;
using HLE.Text;
using OkayegTeaTime.Configuration;
using OkayegTeaTime.Twitch.Models.OpenWeatherMap;

namespace OkayegTeaTime.Twitch.Services;

public sealed class WeatherService
{
    private readonly ConcurrentDoubleDictionary<string, (double Latitude, double Longitude), WeatherData> _weatherCache = new();
    private readonly ConcurrentDictionary<string, ForecastData> _forecastCache = new();

    private static readonly TimeSpan s_cacheTime = TimeSpan.FromMinutes(30);
    private static readonly TimeSpan s_forecastCacheTime = TimeSpan.FromHours(1);

    public async ValueTask<WeatherData?> GetWeatherAsync(string location, bool tryGetFromCache = true)
    {
#pragma warning disable S6354
        if (tryGetFromCache && _weatherCache.TryGetByPrimaryKey(location, out WeatherData? data) && data.TimeOfRequest + s_cacheTime > DateTime.UtcNow)
#pragma warning restore S6354
        {
            return data;
        }

        using HttpClient httpClient = new();
        using HttpResponseMessage response = await httpClient.GetAsync($"https://api.openweathermap.org/data/2.5/weather?q={location}&units=metric&appid={GlobalSettings.Settings.OpenWeatherMap!.ApiKey}");
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        data = await response.Content.ReadFromJsonAsync<WeatherData>();
        if (data is null)
        {
            return null;
        }

        (double Latitude, double Longitude) coordinates = (data.Coordinates.Latitude, data.Coordinates.Longitude);
        if (!_weatherCache.TryAdd(location, coordinates, data))
        {
            _weatherCache[location, coordinates] = data;
        }

        return data;
    }

    public async ValueTask<WeatherData?> GetWeatherAsync(double latitude, double longitude, bool tryGetFromCache = true)
    {
        (double Latitude, double Longitude) key = (latitude, longitude);
#pragma warning disable S6354
        if (tryGetFromCache && _weatherCache.TryGetBySecondaryKey(key, out WeatherData? data) && data.TimeOfRequest + s_cacheTime > DateTime.UtcNow)
#pragma warning restore S6354
        {
            return data;
        }

        using HttpClient httpClient = new();
        using HttpResponseMessage response = await httpClient.GetAsync($"https://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitude}&units=metric&appid={GlobalSettings.Settings.OpenWeatherMap!.ApiKey}");
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        data = await response.Content.ReadFromJsonAsync<WeatherData>();
        if (data is null)
        {
            return null;
        }

        _weatherCache.AddOrSet(data.CityName, key, data);
        return data;
    }

    // ReSharper disable once UnusedMember.Global
    public async ValueTask<ForecastData?> GetForecastAsync(string location, bool tryGetFromCache = true)
    {
#pragma warning disable S6354
        if (tryGetFromCache && _forecastCache.TryGetValue(location, out ForecastData? data) && data.TimeOfRequest + s_forecastCacheTime > DateTime.UtcNow)
#pragma warning restore S6354
        {
            return data;
        }

        using HttpClient httpClient = new();
        using HttpResponseMessage response = await httpClient.GetAsync($"https://api.openweathermap.org/data/2.5/forecast/daily?q={location}&cnt=16&units=metric&appid={GlobalSettings.Settings.OpenWeatherMap!.ApiKey}");
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        data = await response.Content.ReadFromJsonAsync<ForecastData>();
        if (data is null)
        {
            return null;
        }

        if (!_forecastCache.TryAdd(location, data))
        {
            _forecastCache[location] = data;
        }

        return data;
    }

    public static int FormatWeatherData(WeatherData weatherData, PooledStringBuilder response, bool isPrivateLocation)
    {
        if (isPrivateLocation)
        {
            response.Append("(private location)");
        }
        else if (weatherData.Location.Country.Length == 2)
        {
            string country = new RegionInfo(weatherData.Location.Country).EnglishName;
            response.Append($"{weatherData.CityName}, {country}");
        }
        else
        {
            response.Append($"{weatherData.CityName}, {weatherData.Location.Country}");
        }

        response.EnsureCapacity(response.Length + 256);
        response.Append($": {weatherData.WeatherConditions[0].Description} {GetWeatherEmoji(weatherData.WeatherConditions[0].Id)}, ");
        response.Append(weatherData.Weather.Temperature);
        response.Append("°C, min. ");
        response.Append(weatherData.Weather.MinTemperature);
        response.Append("°C, max. ");
        response.Append(weatherData.Weather.MaxTemperature);
        response.Append($"°C, {GetDirection(weatherData.Wind.Direction)} wind speed: ");
        response.Append(weatherData.Wind.Speed);
        response.Append(" m/s, cloud cover: ");
        response.Append(weatherData.Clouds.Percentage);
        response.Append("%, humidity: ");
        response.Append(weatherData.Weather.Humidity);
        response.Append("%, air pressure: ");
        response.Append(weatherData.Weather.Pressure);
        response.Append(" hPa");
        return response.Length;
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

    private static string GetDirection(double degrees) =>
        degrees switch
        {
            < 0 => Emoji.Question,
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
