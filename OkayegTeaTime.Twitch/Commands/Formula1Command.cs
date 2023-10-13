using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE.Emojis;
using HLE.Strings;
using HLE.Twitch.Models;
using OkayegTeaTime.Database;
using OkayegTeaTime.Models.Formula1;
using OkayegTeaTime.Models.OpenWeatherMap;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Twitch.Services;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Formula1, typeof(Formula1Command))]
public readonly struct Formula1Command(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    : IChatCommand<Formula1Command>
{
    public PooledStringBuilder Response { get; } = new(AppSettings.MaxMessageLength);

    public IChatMessage ChatMessage { get; } = chatMessage;

    private readonly TwitchBot _twitchBot = twitchBot;
    private readonly ReadOnlyMemory<char> _prefix = prefix;
    private readonly ReadOnlyMemory<char> _alias = alias;

    private static Race[]? _races;
    private static readonly TimeSpan _nonRaceLength = TimeSpan.FromHours(1);
    private static readonly TimeSpan _raceLength = TimeSpan.FromHours(2);

    public static void Create(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out Formula1Command command)
    {
        command = new(twitchBot, chatMessage, prefix, alias);
    }

    public async ValueTask Handle()
    {
        _races ??= await GetRacesAsync();
        if (_races is null)
        {
            Response.Append(ChatMessage.Username, ", ", Messages.ApiError);
            return;
        }

        Race? race = GetNextOrCurrentRace(_races);
        if (race is null)
        {
            Response.Append(ChatMessage.Username, ", ", Messages.ThereIsNoNextRace);
            return;
        }

        Regex pattern = _twitchBot.MessageRegexCreator.Create(_alias.Span, _prefix.Span, @"\sweather");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            await SendWeatherInformation(race);
            return;
        }

        SendRaceInformation(race);
    }

    private void SendRaceInformation(Race race)
    {
        Span<char> charBuffer = stackalloc char[255];
        DateTime now = DateTime.UtcNow;

        bool raceHasStarted = race.RaceSession.Start > now;
        Response.Append(ChatMessage.Username, ", ", raceHasStarted ? "Next" : "Current", " race: ", race.Name);
        Response.Append(" at the ", race.Circuit.Name, " in ", race.Circuit.Location.Name, ", ", race.Circuit.Location.Country, ". ");
        Response.Append(Emoji.RacingCar, " ");
        if (raceHasStarted)
        {
            Response.Append("The ", race.RaceSession.Name, " will start ");
            Response.Append(race.RaceSession.Start, "R");

            TimeSpan timeBetweenNowAndRaceStart = race.RaceSession.Start - now;
            timeBetweenNowAndRaceStart.TryFormat(charBuffer, out _, "g");
            int indexOfDot = charBuffer.IndexOf('.');
            Response.Append(" (in ", charBuffer[..Unsafe.As<int, Index>(ref indexOfDot)], "). ", Emoji.CheckeredFlag);
        }
        else
        {
            Response.Append("The ", race.RaceSession.Name, " started ");
            Response.Append(race.RaceSession.Start, "t");
            Response.Append(" GMT. ", Emoji.CheckeredFlag);
        }

        Session session = GetNextOrCurrentSession(race);
        if (ReferenceEquals(session, race.RaceSession))
        {
            return;
        }

        if (session.Start > now)
        {
            session.Start.TryFormat(charBuffer, out int length, "R");
            Response.Append("Next session: ", session.Name, ", starting ", charBuffer[..length], " (in ");

            TimeSpan timeBetweenNowAndRaceStart = session.Start - now;
            timeBetweenNowAndRaceStart.TryFormat(charBuffer, out _, "g");
            int indexOfDot = charBuffer.IndexOf('.');
            Response.Append(charBuffer[..indexOfDot], ").");
        }
        else if (session.Start + _nonRaceLength > now)
        {
            session.Start.TryFormat(charBuffer, out int length, "t");
            Response.Append("Current session: ", session.Name, ", started ", charBuffer[..length], " GMT.");
        }
    }

    private async ValueTask SendWeatherInformation(Race race)
    {
        double latitude = double.Parse(race.Circuit.Location.Latitude);
        double longitude = double.Parse(race.Circuit.Location.Longitude);
        WeatherData? weatherData = await _twitchBot.WeatherService.GetWeatherAsync(latitude, longitude, false);
        if (weatherData is null)
        {
            Response.Append(ChatMessage.Username, ", ", Messages.ApiError);
            return;
        }

        weatherData.CityName = race.Circuit.Location.Name;
        weatherData.Location.Country = race.Circuit.Location.Country;
        Response.Append(ChatMessage.Username, ", ");
        int charsWritten = WeatherService.WriteWeatherData(weatherData, Response.FreeBufferSpan, false);
        Response.Advance(charsWritten);
    }

    private static Race? GetNextOrCurrentRace(ReadOnlySpan<Race> races)
    {
        for (int i = 0; i < races.Length; i++)
        {
            Race race = races[i];
            if (race.RaceSession.Start + _raceLength > DateTime.UtcNow)
            {
                return race;
            }
        }

        return null;
    }

    private static Session GetNextOrCurrentSession(Race race)
    {
        Unsafe.SkipInit(out FiveSessionsBuffer sessions);
        if (race.HasSprintRace)
        {
            sessions[0] = race.PracticeOneSession;
            sessions[1] = race.QualifyingSession;
            sessions[2] = race.PracticeTwoSession;
            sessions[3] = race.SprintSession;
            sessions[4] = race.RaceSession;
        }
        else
        {
            sessions[0] = race.PracticeOneSession;
            sessions[1] = race.PracticeTwoSession;
            sessions[2] = race.PracticeThreeSession;
            sessions[3] = race.QualifyingSession;
            sessions[4] = race.RaceSession;
        }

        for (int i = 0; i < 5; i++)
        {
            Session session = sessions[i];
            DateTime sessionEnd = ReferenceEquals(session, race.RaceSession) ? session.Start + _raceLength : session.Start + _nonRaceLength;
            if (sessionEnd > DateTime.UtcNow)
            {
                return session;
            }
        }

        throw new UnreachableException("There has to be a current or next session, if this method has been called");
    }

    private static async ValueTask<Race[]?> GetRacesAsync()
    {
        try
        {
            HttpGet request = await HttpGet.GetStringAsync("https://ergast.com/api/f1/current.json");
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

            await CreateSessionStartTimes(races, jRaces);
            return races;
        }
        catch (Exception ex)
        {
            await DbController.LogExceptionAsync(ex);
            return null;
        }
    }

    private static async ValueTask CreateSessionStartTimes(Race[] races, JsonElement jRaces)
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
            await DbController.LogExceptionAsync(ex);
        }
    }

    public void Dispose()
    {
        Response.Dispose();
    }

    public bool Equals(Formula1Command other)
    {
        return _twitchBot.Equals(other._twitchBot) && _prefix.Equals(other._prefix) && _alias.Equals(other._alias) && Response.Equals(other.Response) && ChatMessage.Equals(other.ChatMessage);
    }

    public override bool Equals(object? obj)
    {
        return obj is Formula1Command other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_twitchBot, _prefix, _alias, Response, ChatMessage);
    }

    public static bool operator ==(Formula1Command left, Formula1Command right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Formula1Command left, Formula1Command right)
    {
        return !left.Equals(right);
    }
}
