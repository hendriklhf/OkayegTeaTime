using System;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE.Text;
using HLE.Twitch.Tmi.Models;
using OkayegTeaTime.Database;
using OkayegTeaTime.Configuration;
using OkayegTeaTime.Twitch.Json;
using OkayegTeaTime.Twitch.Models.Formula1;
using OkayegTeaTime.Twitch.Models.OpenWeatherMap;
using OkayegTeaTime.Twitch.Services;

namespace OkayegTeaTime.Twitch.Commands;

public readonly struct Formula1Command(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    : IChatCommand<Formula1Command>
{
    public PooledStringBuilder Response { get; } = new(GlobalSettings.MaxMessageLength);

    public ChatMessage ChatMessage { get; } = chatMessage;

    private readonly TwitchBot _twitchBot = twitchBot;
    private readonly ReadOnlyMemory<char> _prefix = prefix;
    private readonly ReadOnlyMemory<char> _alias = alias;

    private static Race[]? s_races;
    private static readonly TimeSpan s_nonRaceLength = TimeSpan.FromHours(1);
    private static readonly TimeSpan s_raceLength = TimeSpan.FromHours(2);

    public static void Create(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out Formula1Command command)
        => command = new(twitchBot, chatMessage, prefix, alias);

    public async ValueTask HandleAsync()
    {
#pragma warning disable S2696
        s_races ??= await GetRacesAsync();
#pragma warning restore S2696

        if (s_races is null)
        {
            Response.Append($"{ChatMessage.Username}, {Texts.ApiError}");
            return;
        }

        Race? race = GetNextOrCurrentRace(s_races);
        if (race is null)
        {
            Response.Append($"{ChatMessage.Username}, {Texts.ThereIsNoNextRace}");
            return;
        }

        Regex pattern = _twitchBot.MessageRegexCreator.Create(_alias.Span, _prefix.Span, @"\sweather");
        if (pattern.IsMatch(ChatMessage.Message.AsSpan()))
        {
            if (GlobalSettings.Settings.OpenWeatherMap is null)
            {
                Response.Append($"{ChatMessage.Username}, {Texts.TheCommandHasNotBeenConfiguredByTheBotOwner}");
                return;
            }

            await SendWeatherInformationAsync(race);
            return;
        }

        SendRaceInformation(race);
    }

    private void SendRaceInformation(Race race)
    {
        Span<char> charBuffer = stackalloc char[255];
#pragma warning disable S6354
        DateTime now = DateTime.UtcNow;
#pragma warning restore S6354

        bool raceHasStarted = race.RaceSession.Start <= now;
        Response.Append($"{ChatMessage.Username}, {(raceHasStarted ? "Next" : "Current")} race: {race.Name}");
        Response.Append($" at the {race.Circuit.Name} in {race.Circuit.Location.Name}, {race.Circuit.Location.Country}. {Emoji.RacingCar} ");
        if (!raceHasStarted)
        {
            Response.Append($"The {race.RaceSession.Name} will start {race.RaceSession.Start:R}");

            TimeSpan timeBetweenNowAndRaceStart = race.RaceSession.Start - now;
            timeBetweenNowAndRaceStart.TryFormat(charBuffer, out _, "g");
            int indexOfDot = charBuffer.IndexOf('.');
            Response.Append($" (in {charBuffer[..Unsafe.As<int, Index>(ref indexOfDot)]}). {Emoji.CheckeredFlag}");
        }
        else
        {
            Response.Append($"The {race.RaceSession.Name} started {race.RaceSession.Start:t} GMT. {Emoji.CheckeredFlag}");
        }

        Session session = GetNextOrCurrentSession(race);
        if (ReferenceEquals(session, race.RaceSession))
        {
            return;
        }

        if (session.Start > now)
        {
            session.Start.TryFormat(charBuffer, out int length, "R");
            Response.Append($"Next session: {session.Name}, starting {charBuffer[..length]} (in ");

            TimeSpan timeBetweenNowAndRaceStart = session.Start - now;
            timeBetweenNowAndRaceStart.TryFormat(charBuffer, out _, "g");
            int indexOfDot = charBuffer.IndexOf('.');
            Response.Append($"{charBuffer[..indexOfDot]}).");
        }
        else if (session.Start + s_nonRaceLength > now)
        {
            session.Start.TryFormat(charBuffer, out int length, "t");
            Response.Append($"Current session: {session.Name}, started {charBuffer[..length]} GMT.");
        }
        else
        {
            Debug.Fail("this should not be possible");
        }
    }

    private async ValueTask SendWeatherInformationAsync(Race race)
    {
        double latitude = double.Parse(race.Circuit.Location.Latitude, CultureInfo.InvariantCulture);
        double longitude = double.Parse(race.Circuit.Location.Longitude, CultureInfo.InvariantCulture);
        WeatherData? weatherData = await _twitchBot.WeatherService.GetWeatherAsync(latitude, longitude, false);
        if (weatherData is null)
        {
            Response.Append($"{ChatMessage.Username}, {Texts.ApiError}");
            return;
        }

        weatherData.CityName = race.Circuit.Location.Name;
        weatherData.Location.Country = race.Circuit.Location.Country;
        Response.Append($"{ChatMessage.Username}, ");
        int charsWritten = WeatherService.FormatWeatherData(weatherData, Response, false);
        Response.Advance(charsWritten);
    }

    private static Race? GetNextOrCurrentRace(ReadOnlySpan<Race> races)
    {
        for (int i = 0; i < races.Length; i++)
        {
            Race race = races[i];
#pragma warning disable S6354
            if (race.RaceSession.Start + s_raceLength > DateTime.UtcNow)
#pragma warning restore S6354
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
        }
        else
        {
            sessions[0] = race.PracticeOneSession;
            sessions[1] = race.PracticeTwoSession;
            sessions[2] = race.PracticeThreeSession;
            sessions[3] = race.QualifyingSession;
        }

        sessions[4] = race.RaceSession;

        for (int i = 0; i < 5; i++)
        {
            Session session = sessions[i];
            DateTime sessionEnd = ReferenceEquals(session, race.RaceSession) ? session.Start + s_raceLength : session.Start + s_nonRaceLength;
#pragma warning disable S6354
            if (sessionEnd > DateTime.UtcNow)
#pragma warning restore S6354
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
            using HttpClient httpClient = new();
            using HttpResponseMessage response = await httpClient.GetAsync("https://ergast.com/api/f1/current.json");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            JsonElement json = await response.Content.ReadFromJsonAsync<JsonElement>();
            JsonElement jsonRaces = json.GetProperty("MRData").GetProperty("RaceTable").GetProperty("Races");
            Race[]? races = JsonSerializer.Deserialize(jsonRaces.GetRawText(), Formula1JsonSerializerContext.Default.RaceArray);
            if (races is null)
            {
                return null;
            }

            await CreateSessionStartTimesAsync(races, jsonRaces);
            return races;
        }
        catch (Exception ex)
        {
            await DbController.LogExceptionAsync(ex);
            return null;
        }
    }

    private static async ValueTask CreateSessionStartTimesAsync(Race[] races, JsonElement jRaces)
    {
        try
        {
            (string RaceProperty, string? JsonProperty, string SessionName)[] props =
            [
                (nameof(Race.PracticeOneSession), "FirstPractice", "Free practice 1"),
                (nameof(Race.PracticeTwoSession), "SecondPractice", "Free practice 2"),
                (nameof(Race.PracticeThreeSession), "ThirdPractice", "Free practice 3"),
                (nameof(Race.QualifyingSession), "Qualifying", "Qualifying"),
                (nameof(Race.SprintSession), "Sprint", "Sprint race"),
                (nameof(Race.RaceSession), null, "Grand Prix")
            ];

            for (int i = 0; i < races.Length; i++)
            {
                foreach ((string RaceProperty, string? JsonProperty, string SessionName) prop in props)
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
                    else
                    {
                        Debug.Fail("this should not be possible");
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

    public void Dispose() => Response.Dispose();

    public bool Equals(Formula1Command other) =>
        _twitchBot.Equals(other._twitchBot) && _prefix.Equals(other._prefix) && _alias.Equals(other._alias) &&
        Response.Equals(other.Response) && ChatMessage.Equals(other.ChatMessage);

    public override bool Equals(object? obj) => obj is Formula1Command other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(_twitchBot, _prefix, _alias, Response, ChatMessage);

    public static bool operator ==(Formula1Command left, Formula1Command right) => left.Equals(right);

    public static bool operator !=(Formula1Command left, Formula1Command right) => !left.Equals(right);
}
