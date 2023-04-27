using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using HLE.Collections;
using HLE.Emojis;
using HLE.Strings;
using OkayegTeaTime.Models.OpenWeatherMap;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Utils;

#pragma warning disable CS0659

namespace OkayegTeaTime.Twitch.Controller;

public sealed class WeatherController
{
    private readonly DoubleDictionary<int, (double, double), WeatherData> _weatherCache = new();
    private readonly Dictionary<int, ForecastData> _forecastCache = new();
    private readonly TimeSpan _cacheTime = TimeSpan.FromMinutes(30);
    private readonly TimeSpan _forecastCacheTime = TimeSpan.FromHours(1);

    public WeatherData? GetWeather(ReadOnlySpan<char> city, bool loadFromCache = true)
    {
        int key = string.GetHashCode(city, StringComparison.OrdinalIgnoreCase);
        if (loadFromCache && _weatherCache.TryGetValue(key, out WeatherData? data) && data.TimeOfRequest + _cacheTime > DateTime.UtcNow)
        {
            return data;
        }

        HttpGet request = new($"https://api.openweathermap.org/data/2.5/weather?q={city}&units=metric&appid={AppSettings.OpenWeatherMapApiKey}");
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

    public WeatherData? GetWeather(int latitude, int longitude, bool loadFromCache = true)
    {
        var key = ((double)latitude, (double)longitude);
        if (loadFromCache && _weatherCache.TryGetValue(key, out WeatherData? data) && data.TimeOfRequest + _cacheTime > DateTime.UtcNow)
        {
            return data;
        }

        HttpGet request = new($"https://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitude}&units=metric&appid={AppSettings.OpenWeatherMapApiKey}");
        if (request.Result is null)
        {
            return null;
        }

        data = JsonSerializer.Deserialize<WeatherData>(request.Result);
        if (data is null)
        {
            return null;
        }

        int locationHash = string.GetHashCode(data.CityName);
        if (!_weatherCache.TryAdd(locationHash, key, data))
        {
            _weatherCache[locationHash, key] = data;
        }

        return data;
    }

    // ReSharper disable once UnusedMember.Global
    public ForecastData? GetForecast(ReadOnlySpan<char> city, bool loadFromCache = true)
    {
        int key = string.GetHashCode(city, StringComparison.OrdinalIgnoreCase);
        if (loadFromCache && _forecastCache.TryGetValue(key, out ForecastData? data) && data.TimeOfRequest + _forecastCacheTime > DateTime.UtcNow)
        {
            return data;
        }

        HttpGet request = new($"https://api.openweathermap.org/data/2.5/forecast/daily?q={city}&cnt=16&units=metric&appid={AppSettings.OpenWeatherMapApiKey}");
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

    public static void WriteResponse(WeatherData weatherData, ref PoolBufferStringBuilder response, bool isPrivateLocation)
    {
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
        response.Append("°C, ", GetDirection(weatherData.Wind.Direction), "wind speed: ");
        response.Append(weatherData.Wind.Speed);
        response.Append(" m/s, cloud cover: ");
        response.Append(weatherData.Clouds.Percentage);
        response.Append("%, humidity: ");
        response.Append(weatherData.Weather.Humidity);
        response.Append("%, air pressure: ");
        response.Append(weatherData.Weather.Pressure);
        response.Append(" hPa");
    }

    private static string GetWeatherEmoji(int weatherId)
    {
        return weatherId switch
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
    }

    private static string GetDirection(double degrees)
    {
        return degrees switch
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
}
