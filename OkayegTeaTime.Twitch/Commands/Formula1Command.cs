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
using StringHelper = HLE.StringHelper;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Formula1)]
public readonly unsafe ref struct Formula1Command
{
    public TwitchChatMessage ChatMessage { get; }

    public Response* Response { get; }

    private readonly TwitchBot _twitchBot;
    private readonly string? _prefix;
    private readonly string _alias;

    private static Formula1Race[]? _races;
    private static readonly TimeSpan _nonRaceLength = TimeSpan.FromHours(1);
    private static readonly TimeSpan _raceLength = TimeSpan.FromHours(2);

    public Formula1Command(TwitchBot twitchBot, TwitchChatMessage chatMessage, Response* response, string? prefix, string alias)
    {
        ChatMessage = chatMessage;
        Response = response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
        _races ??= GetRaces();
    }

    public void Handle()
    {
        if (_races is null)
        {
            Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, PredefinedMessages.ApiError);
            return;
        }

        Formula1Race? race = GetNextOrCurrentRace(_races);
        if (race is null)
        {
            Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, PredefinedMessages.ThereIsNoNextRace);
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
                Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, PredefinedMessages.ApiError);
                return;
            }

            weatherData.CityName = race.Circuit.Location.Name;
            weatherData.Location.Country = race.Circuit.Location.Country;
            Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, WeatherController.CreateResponse(weatherData, false));
        }
        else
        {
            Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, race.Race.Start > DateTime.UtcNow ? "Next" : "Current", "race: ", race.Name);
            Response->Append(" at the ", race.Circuit.Name, " in ", race.Circuit.Location.Name, PredefinedMessages.CommaSpace, race.Circuit.Location.Country, ". ");
            Response->Append(Emoji.RacingCar, StringHelper.Whitespace);
            if (race.Race.Start > DateTime.UtcNow)
            {
                TimeSpan timeBetweenNowAndRaceStart = race.Race.Start - DateTime.UtcNow;
                Response->Append("The ", race.Race.Name, " will start on ", race.Race.Start.ToString("R"));
                Response->Append(" (in ", timeBetweenNowAndRaceStart.ToString("g").Split('.')[0], "). ", Emoji.CheckeredFlag);
            }
            else
            {
                Response->Append("The ", race.Race.Name, " started ", race.Race.Start.ToString("t"), " GMT. ", Emoji.CheckeredFlag);
            }

            Formula1Session session = GetNextOrCurrentSession(race);
            if (ReferenceEquals(session, race.Race))
            {
                return;
            }

            if (session.Start > DateTime.UtcNow)
            {
                TimeSpan timeBetweenNowAndRaceStart = session.Start - DateTime.UtcNow;
                Response->Append("Next session: ", session.Name, ", starting on ", session.Start.ToString("R"), "(in ", timeBetweenNowAndRaceStart.ToString("g").Split('.')[0], ").");
            }
            else if (session.Start + _nonRaceLength > DateTime.UtcNow)
            {
                Response->Append("Current session: ", session.Name, ", started ", session.Start.ToString("t"));
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

        return sessions.First(s => (ReferenceEquals(s, race.Race) ? s.Start + _raceLength : s.Start + _nonRaceLength) > DateTime.UtcNow);
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

            CreateSessionStartTimes(races, jRaces);
            return races;
        }
        catch (Exception ex)
        {
            DbController.LogException(ex);
            return null;
        }
    }

    private static void CreateSessionStartTimes(Formula1Race[] races, JsonElement jRaces)
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
