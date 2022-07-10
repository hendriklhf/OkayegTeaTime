using System;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using HLE.Emojis;
using HLE.Http;
using OkayegTeaTime.Database;
using OkayegTeaTime.Files.Models;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

public class Formula1Command : Command
{
    private static Formula1Race[]? _races;

    public Formula1Command(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        _races ??= GetRaces();
        if (_races is null)
        {
            Response = $"{ChatMessage.Username}, api error";
            return;
        }

        Formula1Race? race = GetNextRace(_races);
        if (race is null)
        {
            Response = $"{ChatMessage.Username}, there is no next race";
            return;
        }

        Regex pattern = PatternCreator.Create(_alias, _prefix, @"\s--weather");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            int latitude = (int)Math.Round(double.Parse(race.Circuit.Location.Latitude));
            int longitude = (int)Math.Round(double.Parse(race.Circuit.Location.Longitude));
            OpenWeatherMapResponse? weatherData = _twitchBot.WeatherController.GetWeatherData(latitude, longitude, false);
            if (weatherData is null)
            {
                Response = $"{ChatMessage.Username}, api error";
                return;
            }

            weatherData.CityName = race.Circuit.Location.Name;
            weatherData.Location.Country = race.Circuit.Location.Country;
            Response = $"{ChatMessage.Username}, {_twitchBot.WeatherController.ToResponse(weatherData, false)}";
        }
        else
        {
            Response = $"{Emoji.RacingCar} Next race: {race.Racename} at the {race.Circuit.Name} in {race.Circuit.Location.Name}, {race.Circuit.Location.Country}. ";
            Formula1Session nextSession = GetNextSession(race);
            TimeSpan ts;
            if (nextSession != race.Race)
            {
                ts = nextSession.Start - DateTime.UtcNow;
                Response += $"Next session: {nextSession.Name}, starting on {nextSession.Start:R} (in {ts.ToString("g").Split('.')[0]}). ";
            }

            ts = race.Race.Start - DateTime.UtcNow;
            Response += $"The {race.Race.Name} will start on {race.Race.Start:R} (in {ts.ToString("g").Split('.')[0]}). {Emoji.CheckeredFlag} ";
        }
    }

    private static Formula1Race? GetNextRace(Formula1Race[] races)
    {
        return races.FirstOrDefault(r => r.Race.Start > DateTime.UtcNow);
    }

    private static Formula1Session GetNextSession(Formula1Race race)
    {
        Formula1Session[] sessions = race.HasSprintRace switch
        {
            true => new[]
            {
                race.PracticeOne,
                race.Qualifying,
                race.PracticeTwo,
                race.Sprint,
                race.Race
            },
            _ => new[]
            {
                race.PracticeOne,
                race.PracticeTwo,
                race.PracticeThree,
                race.Qualifying,
                race.Race
            }
        };

        return sessions.First(s => s.Start > DateTime.UtcNow);
    }

    private static Formula1Race[]? GetRaces()
    {
        try
        {
            HttpGet request = new("https://ergast.com/api/f1/current.json");
            if (request.Result is null || !request.IsValidJsonData)
            {
                return null;
            }

            JsonElement jRaces = request.Data.GetProperty("MRData").GetProperty("RaceTable").GetProperty("Races");
            Formula1Race[]? races = JsonSerializer.Deserialize<Formula1Race[]?>(jRaces.GetRawText());
            if (races is null)
            {
                return null;
            }

            GetDateTimes(races, jRaces);
            return races;
        }
        catch (Exception ex)
        {
            DbController.LogException(ex);
            return null;
        }
    }

    private static void GetDateTimes(Formula1Race[] races, JsonElement jRaces)
    {
        try
        {
            (string SessionProperty, string? JsonProperty, string SessionName)[] props =
            {
                (nameof(Formula1Race.PracticeOne), "FirstPractice", "Free practice 1"),
                (nameof(Formula1Race.PracticeTwo), "SecondPractice", "Free practice 2"),
                (nameof(Formula1Race.PracticeThree), "ThirdPractice", "Free practice 3"),
                (nameof(Formula1Race.Qualifying), "Qualifying", "Qualifying"),
                (nameof(Formula1Race.Sprint), "Sprint", "Sprint race"),
                (nameof(Formula1Race.Race), null, "Grand Prix")
            };

            for (int i = 0; i < races.Length; i++)
            {
                foreach (var prop in props)
                {
                    JsonElement jProp;
                    if (prop.JsonProperty is null)
                    {
                        jProp = jRaces[i];
                    }
                    else if (!jRaces[i].TryGetProperty(prop.JsonProperty, out jProp))
                    {
                        continue;
                    }

                    JsonElement jDate = jProp.GetProperty("date");
                    JsonElement jTime = jProp.GetProperty("time");
                    string? date = jDate.GetString();
                    string? time = jTime.GetString();
                    if (date is null || time is null)
                    {
                        throw new InvalidOperationException($"{nameof(date)} is {date ?? "null"} and {nameof(time)} is {time ?? "null"}");
                    }

                    DateTime startTime = DateTime.Parse($"{date}T{time}").ToUniversalTime();
                    Formula1Session session = new(prop.SessionName, startTime);
                    typeof(Formula1Race).GetProperty(prop.SessionProperty)!.SetValue(races[i], session);
                }
            }
        }
        catch (Exception ex)
        {
            DbController.LogException(ex);
        }
    }
}
