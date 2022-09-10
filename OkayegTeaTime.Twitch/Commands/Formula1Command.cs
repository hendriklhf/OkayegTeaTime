using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using HLE.Emojis;
using HLE.Http;
using OkayegTeaTime.Database;
using OkayegTeaTime.Files.Models;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Controller;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Formula1)]
public class Formula1Command : Command
{
    private static Formula1Race[]? _races;
    private static readonly TimeSpan _nonRaceLength = TimeSpan.FromHours(1);
    private static readonly TimeSpan _raceLength = TimeSpan.FromHours(2);

    public Formula1Command(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias) : base(twitchBot, chatMessage, alias)
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

        Formula1Race? race = GetNextOrCurrentRace(_races);
        if (race is null)
        {
            Response = $"{ChatMessage.Username}, there is no next race";
            return;
        }

        Regex pattern = PatternCreator.Create(_alias, _prefix, @"\sweather");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            int latitude = (int)Math.Round(double.Parse(race.Circuit.Location.Latitude));
            int longitude = (int)Math.Round(double.Parse(race.Circuit.Location.Longitude));
            OwmWeatherData? weatherData = _twitchBot.WeatherController.GetWeather(latitude, longitude, false);
            if (weatherData is null)
            {
                Response = $"{ChatMessage.Username}, api error";
                return;
            }

            weatherData.CityName = race.Circuit.Location.Name;
            weatherData.Location.Country = race.Circuit.Location.Country;
            Response = $"{ChatMessage.Username}, {WeatherController.CreateResponse(weatherData, false)}";
        }
        else
        {
            Response = $"{ChatMessage.Username}, {(race.Race.Start > DateTime.UtcNow ? "Next" : "Current")} race: " +
                $"{race.Racename} at the {race.Circuit.Name} in {race.Circuit.Location.Name}, {race.Circuit.Location.Country}. {Emoji.RacingCar} ";
            if (race.Race.Start > DateTime.UtcNow)
            {
                TimeSpan ts = race.Race.Start - DateTime.UtcNow;
                Response += $"The {race.Race.Name} will start on {race.Race.Start:R} (in {ts.ToString("g").Split('.')[0]}). {Emoji.CheckeredFlag} ";
            }
            else
            {
                Response += $"The {race.Race.Name} started {race.Race.Start:t} GMT. {Emoji.CheckeredFlag}";
            }

            Formula1Session session = GetNextOrCurrentSession(race);
            if (session == race.Race)
            {
                return;
            }

            if (session.Start > DateTime.UtcNow)
            {
                TimeSpan ts = session.Start - DateTime.UtcNow;
                Response += $"Next session: {session.Name}, starting on {session.Start:R} (in {ts.ToString("g").Split('.')[0]}).";
            }
            else if (session.Start + _nonRaceLength > DateTime.UtcNow)
            {
                Response += $"Current session: {session.Name}, started {session.Start:t}.";
            }
        }
    }

    private static Formula1Race? GetNextOrCurrentRace(IEnumerable<Formula1Race> races)
    {
        return races.FirstOrDefault(r => r.Race.Start + _raceLength > DateTime.UtcNow);
    }

    private static Formula1Session GetNextOrCurrentSession(Formula1Race race)
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

        return sessions.First(s => (s == race.Race ? s.Start + _raceLength : s.Start + _nonRaceLength) > DateTime.UtcNow);
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
            (string RaceProperty, string? JsonProperty, string SessionName)[] props =
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
                    typeof(Formula1Race).GetProperty(prop.RaceProperty)!.SetValue(races[i], session);
                }
            }
        }
        catch (Exception ex)
        {
            DbController.LogException(ex);
        }
    }
}