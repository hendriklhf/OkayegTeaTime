using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using HLE;
using HLE.Emojis;
using HLE.Twitch.Models;
using OkayegTeaTime.Database;
using OkayegTeaTime.Models.Formula1;
using OkayegTeaTime.Models.OpenWeatherMap;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Controller;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;
using StringHelper = HLE.StringHelper;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Formula1)]
public readonly unsafe ref struct Formula1Command
{
    public ChatMessage ChatMessage { get; }

    public StringBuilder* Response { get; }

    private readonly TwitchBot _twitchBot;
    private readonly string? _prefix;
    private readonly string _alias;

    private static Race[]? _races;
    private static readonly TimeSpan _nonRaceLength = TimeSpan.FromHours(1);
    private static readonly TimeSpan _raceLength = TimeSpan.FromHours(2);

    public Formula1Command(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
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
            Response->Append(ChatMessage.Username, Messages.CommaSpace, Messages.ApiError);
            return;
        }

        Race? race = GetNextOrCurrentRace(_races);
        if (race is null)
        {
            Response->Append(ChatMessage.Username, Messages.CommaSpace, Messages.ThereIsNoNextRace);
            return;
        }

        Regex pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\sweather");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            SendWeatherInformation(race);
        }
        else
        {
            SendRaceInformation(race);
        }
    }

    private void SendRaceInformation(Race race)
    {
        Response->Append(ChatMessage.Username, Messages.CommaSpace, race.RaceSession.Start > DateTime.UtcNow ? "Next" : "Current", "race: ", race.Name);
        Response->Append(" at the ", race.Circuit.Name, " in ", race.Circuit.Location.Name, Messages.CommaSpace, race.Circuit.Location.Country, ". ");
        Response->Append(Emoji.RacingCar, StringHelper.Whitespace);
        if (race.RaceSession.Start > DateTime.UtcNow)
        {
            TimeSpan timeBetweenNowAndRaceStart = race.RaceSession.Start - DateTime.UtcNow;
            Response->Append("The ", race.RaceSession.Name, " will start on ", race.RaceSession.Start.ToString("R"));
            Response->Append(" (in ", timeBetweenNowAndRaceStart.ToString("g").Split('.')[0], "). ", Emoji.CheckeredFlag);
        }
        else
        {
            Response->Append("The ", race.RaceSession.Name, " started ", race.RaceSession.Start.ToString("t"), " GMT. ", Emoji.CheckeredFlag);
        }

        Session session = GetNextOrCurrentSession(race);
        if (ReferenceEquals(session, race.RaceSession))
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

    private void SendWeatherInformation(Race race)
    {
        int latitude = (int)Math.Round(double.Parse(race.Circuit.Location.Latitude));
        int longitude = (int)Math.Round(double.Parse(race.Circuit.Location.Longitude));
        WeatherData? weatherData = _twitchBot.WeatherController.GetWeather(latitude, longitude, false);
        if (weatherData is null)
        {
            Response->Append(ChatMessage.Username, Messages.CommaSpace, Messages.ApiError);
            return;
        }

        weatherData.CityName = race.Circuit.Location.Name;
        weatherData.Location.Country = race.Circuit.Location.Country;
        Response->Append(ChatMessage.Username, Messages.CommaSpace, WeatherController.CreateResponse(weatherData, false));
    }

    private static Race? GetNextOrCurrentRace(IEnumerable<Race> races)
    {
        return races.FirstOrDefault(r => r.RaceSession.Start + _raceLength > DateTime.UtcNow);
    }

    private static Session GetNextOrCurrentSession(Race race)
    {
        Session[] sessions = race.HasSprintRace switch
        {
            true => new[]
            {
                race.PracticeOneSession,
                race.QualifyingSession,
                race.PracticeTwoSession,
                race.SprintSession,
                race.RaceSession
            },
            _ => new[]
            {
                race.PracticeOneSession,
                race.PracticeTwoSession,
                race.PracticeThreeSession,
                race.QualifyingSession,
                race.RaceSession
            }
        };

        return sessions.First(s => (ReferenceEquals(s, race.RaceSession) ? s.Start + _raceLength : s.Start + _nonRaceLength) > DateTime.UtcNow);
    }

    private static Race[]? GetRaces()
    {
        try
        {
            HttpGet request = new("https://ergast.com/api/f1/current.json");
            if (request.Result is null)
            {
                return null;
            }

            JsonElement json = JsonSerializer.Deserialize<JsonElement>(request.Result);
            JsonElement jRaces = json.GetProperty("MRData").GetProperty("RaceTable").GetProperty("Races");
            Race[]? races = JsonSerializer.Deserialize<Race[]?>(jRaces.GetRawText());
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

    private static void CreateSessionStartTimes(Race[] races, JsonElement jRaces)
    {
        try
        {
            (string RaceProperty, string? JsonProperty, string SessionName)[] props =
            {
                (nameof(Race.PracticeOneSession), "FirstPractice", "Free practice 1"),
                (nameof(Race.PracticeTwoSession), "SecondPractice", "Free practice 2"),
                (nameof(Race.PracticeThreeSession), "ThirdPractice", "Free practice 3"),
                (nameof(Race.QualifyingSession), "Qualifying", "Qualifying"),
                (nameof(Race.SprintSession), "Sprint", "Sprint race"),
                (nameof(Race.RaceSession), null, "Grand Prix")
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
                    Session session = new(prop.SessionName, startTime);
                    typeof(Race).GetProperty(prop.RaceProperty)!.SetValue(races[i], session);
                }
            }
        }
        catch (Exception ex)
        {
            DbController.LogException(ex);
        }
    }
}
