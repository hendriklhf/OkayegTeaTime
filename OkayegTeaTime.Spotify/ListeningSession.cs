using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using OkayegTeaTime.Database.Models;

namespace OkayegTeaTime.Spotify;

public sealed class ListeningSession
{
    public SpotifyUser Host { get; }

    public List<SpotifyUser> Listeners { get; } = new();

    private readonly Timer _timer = new();

    public ListeningSession(SpotifyUser host)
    {
        Host = host;
        _timer.Elapsed += async (_, _) => await Timer_OnElapsed();
    }

    public void StartTimer(double interval)
    {
        _timer.Stop();
        _timer.Interval = interval;
        _timer.Start();
    }

    public void StopTimer()
    {
        _timer.Stop();
    }

    private async Task Timer_OnElapsed()
    {
        SpotifyItem? song;
        try
        {
            song = await SpotifyController.GetCurrentlyPlayingItemAsync(Host);
        }
        catch (SpotifyException)
        {
            Listeners.Clear();
            _timer.Stop();
            return;
        }

        if (song is null)
        {
            Listeners.Clear();
            _timer.Stop();
            return;
        }

        _timer.Interval = song.Duration;
        _timer.Start();

        List<SpotifyUser> usersToRemove = new();
        foreach (SpotifyUser listener in Listeners)
        {
            try
            {
                await SpotifyController.ListenToAsync(listener, song);
            }
            catch (SpotifyException)
            {
                usersToRemove.Add(listener);
            }
        }

        foreach (SpotifyUser utr in usersToRemove)
        {
            Listeners.Remove(utr);
        }
    }
}
