using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Text.Json;
using System.Threading.Tasks;
using HLE.Collections.Concurrent;
using HLE.Emojis;
using HLE.Strings;
using OkayegTeaTime.Models.OpenWeatherMap;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Services;

public sealed class WeatherService : IDisposable
{
    private readonly ConcurrentDoubleDictionary<int, (double, double), WeatherData> _weatherCache = new();
    private readonly ConcurrentDictionary<int, ForecastData> _forecastCache = new();
    private readonly TimeSpan _cacheTime = TimeSpan.FromMinutes(30);
    private readonly TimeSpan _forecastCacheTime = TimeSpan.FromHours(1);

    public async ValueTask<WeatherData?> GetWeatherAsync(string location, bool tryGetFromCache = true)
    {
        int key = string.GetHashCode(location, StringComparison.OrdinalIgnoreCase);
        if (tryGetFromCache && _weatherCache.TryGetByPrimaryKey(key, out WeatherData? data) && data.TimeOfRequest + _cacheTime > DateTime.UtcNow)
        {
            return data;
        }

        HttpGet request = await HttpGet.GetStringAsync($"https://api.openweathermap.org/data/2.5/weather?q={location}&units=metric&appid={AppSettings.OpenWeatherMapApiKey}");
        if (request.Result is null)
        {
            return null;
        }

        data = JsonSerializer.Deserialize<WeatherData>(request.Result);
        if (data is null)
        {
            return null;
        }

        var coordinates = (data.Coordinates.Latitude, data.Coordinates.Longitude);
        if (!_weatherCache.TryAdd(key, coordinates, data))
        {
            _weatherCache[key, coordinates] = data;
        }

        return data;
    }

    public async ValueTask<WeatherData?> GetWeatherAsync(double latitude, double longitude, bool tryGetFromCache = true)
    {
        var key = (latitude, longitude);
        if (tryGetFromCache && _weatherCache.TryGetBySecondaryKey(key, out WeatherData? data) && data.TimeOfRequest + _cacheTime > DateTime.UtcNow)
        {
            return data;
        }

        HttpGet request = await HttpGet.GetStringAsync($"https://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitude}&units=metric&appid={AppSettings.OpenWeatherMapApiKey}");
        if (request.Result is null)
        {
            return null;
        }

        data = JsonSerializer.Deserialize<WeatherData>(request.Result);
        if (data is null)
        {
            return null;
        }

        int locationHash = string.GetHashCode(data.CityName, StringComparison.OrdinalIgnoreCase);
        _weatherCache.AddOrSet(locationHash, key, data);
        return data;
    }

    // ReSharper disable once UnusedMember.Global
    public async ValueTask<ForecastData?> GetForecast(string city, bool tryGetFromCache = true)
    {
        int key = string.GetHashCode(city, StringComparison.OrdinalIgnoreCase);
        if (tryGetFromCache && _forecastCache.TryGetValue(key, out ForecastData? data) && data.TimeOfRequest + _forecastCacheTime > DateTime.UtcNow)
        {
            return data;
        }

        HttpGet request = await HttpGet.GetStringAsync($"https://api.openweathermap.org/data/2.5/forecast/daily?q={city}&cnt=16&units=metric&appid={AppSettings.OpenWeatherMapApiKey}");
        if (request.Result is null)
        {
            return null;
        }

        data = JsonSerializer.Deserialize<ForecastData>(request.Result);
        if (data is null)
        {
            return null;
        }

        if (!_forecastCache.TryAdd(key, data))
        {
            _forecastCache[key] = data;
        }

        return data;
    }

    public static int WriteWeatherData(WeatherData weatherData, Span<char> responseBuffer, bool isPrivateLocation)
    {
        ValueStringBuilder response = new(responseBuffer);

        if (isPrivateLocation)
        {
            response.Append("(private location)");
        }
        else if (weatherData.Location.Country.Length == 2)
        {
            string country = new RegionInfo(weatherData.Location.Country).EnglishName;
            response.Append(weatherData.CityName, ", ", country);
        }
        else
        {
            response.Append(weatherData.CityName, ", ", weatherData.Location.Country);
        }

        response.Append(": ", weatherData.WeatherConditions[0].Description, " ", GetWeatherEmoji(weatherData.WeatherConditions[0].Id), ", ");
        response.Append(weatherData.Weather.Temperature);
        response.Append("°C, min. ");
        response.Append(weatherData.Weather.MinTemperature);
        response.Append("°C, max. ");
        response.Append(weatherData.Weather.MaxTemperature);
        response.Append("°C, ", GetDirection(weatherData.Wind.Direction), " wind speed: ");
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

    public void Dispose() => _weatherCache.Dispose();
}
