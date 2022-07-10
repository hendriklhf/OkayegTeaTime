using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using HLE.Http;
using OkayegTeaTime.Database;
using OkayegTeaTime.Files.Models;
using OkayegTeaTime.Twitch.Models;

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

        Response += $"Next race: {race.Racename}. ";
        Formula1Session nextSession = GetNextSession(race);
        TimeSpan timeUntil = nextSession.Start - DateTime.UtcNow;
        Response += $"Next session: {nextSession.Name}, starting on {nextSession.Start:R} (in {timeUntil.ToString("g").Split('.')[0]}). ";
        if (nextSession != race.Race)
        {
            timeUntil = race.Race.Start - DateTime.UtcNow;
            Response += $"The {race.Race.Name} will start on {race.Race.Start:R} (in {timeUntil.ToString("g").Split('.')[0]}). ";
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

            JsonElement racesJArray = request.Data.GetProperty("MRData").GetProperty("RaceTable").GetProperty("Races");
            Formula1Race[]? races = JsonSerializer.Deserialize<Formula1Race[]?>(racesJArray.GetRawText());
            if (races is null)
            {
                return null;
            }

            GetDateTimes(races, racesJArray);
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
            MethodInfo tryGetProperty = typeof(JsonElement).GetMethod(nameof(JsonElement.TryGetProperty), new[]
            {
                typeof(string),
                typeof(JsonElement).MakeByRefType()
            })!;
            MethodInfo getString = typeof(JsonElement).GetMethod(nameof(JsonElement.GetString))!;
            MethodInfo dateTimeParse = typeof(DateTime).GetMethod(nameof(DateTime.Parse), new[]
            {
                typeof(string)
            })!;
            (string SessionProperty, string? JsonProperty, string SessionName)[] jsonProps =
            {
                (nameof(Formula1Race.PracticeOne), "FirstPractice", "Free practice 1"),
                (nameof(Formula1Race.PracticeTwo), "SecondPractice", "Free practice 2"),
                (nameof(Formula1Race.PracticeThree), "ThirdPractice", "Free practice 3"),
                (nameof(Formula1Race.Qualifying), "Qualifying", "Qualifying"),
                (nameof(Formula1Race.Sprint), "Sprint", "Sprint race"),
                (nameof(Formula1Race.Race), null, "Grand Prix")
            };
            ConstructorInfo sessionContructor = typeof(Formula1Session).GetConstructor(new[]
            {
                typeof(string),
                typeof(DateTime)
            })!;

            for (int i = 0; i < races.Length; i++)
            {
                foreach (var prop in jsonProps)
                {
                    JsonElement jProp;
                    if (prop.JsonProperty is null)
                    {
                        jProp = jRaces[i];
                    }
                    else
                    {
                        object[] parameters =
                        {
                            prop.JsonProperty,
                            default(JsonElement)
                        };
                        bool propSuccess = (bool)tryGetProperty.Invoke(jRaces[i], parameters)!;
                        if (!propSuccess)
                        {
                            continue;
                        }

                        jProp = (JsonElement)parameters[1];
                    }

                    object[] params1 =
                    {
                        "date",
                        default(JsonElement)
                    };
                    bool dateSuccess = (bool)tryGetProperty.Invoke(jProp, params1)!;
                    object[] params2 =
                    {
                        "time",
                        default(JsonElement)
                    };
                    bool timeSuccess = (bool)tryGetProperty.Invoke(jProp, params2)!;
                    if (!dateSuccess || !timeSuccess)
                    {
                        throw new InvalidOperationException($"{nameof(dateSuccess)} is {dateSuccess} and {nameof(timeSuccess)} is {timeSuccess}");
                    }

                    JsonElement jDate = (JsonElement)params1[1];
                    JsonElement jTime = (JsonElement)params2[1];
                    string? date = (string?)getString.Invoke(jDate, null);
                    string? time = (string?)getString.Invoke(jTime, null);
                    if (date is null || time is null)
                    {
                        throw new InvalidOperationException($"{nameof(date)} is {date ?? "null"} and {nameof(time)} is {time ?? "null"}");
                    }

                    DateTime startTime = ((DateTime)dateTimeParse.Invoke(null, new object[]
                    {
                        $"{date}T{time}"
                    })!).ToUniversalTime();
                    Formula1Session session = (Formula1Session)sessionContructor.Invoke(new object[]
                    {
                        prop.SessionName,
                        startTime
                    });
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
