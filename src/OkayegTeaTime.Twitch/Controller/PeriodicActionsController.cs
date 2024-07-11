using System;
using System.Threading;
using System.Threading.Tasks;
using OkayegTeaTime.Database;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Controller;

public sealed class PeriodicActionsController(PeriodicAction[] periodicActions) : IDisposable
{
    private readonly PeriodicAction[] _periodicActions = periodicActions;
    private CancellationTokenSource? _cancellationTokenSource;

    public void StartAll()
    {
        try
        {
            _cancellationTokenSource ??= new();
            _ = Parallel.ForEachAsync(_periodicActions, _cancellationTokenSource.Token, static async (periodicAction, cancellationToken) =>
            {
                try
                {
                    using PeriodicTimer timer = new(periodicAction.Interval);
                    while (!cancellationToken.IsCancellationRequested && await timer.WaitForNextTickAsync(cancellationToken))
                    {
                        await periodicAction.Action();
                    }
                }
                catch (Exception ex)
                {
                    await DbController.LogExceptionAsync(ex);
                }
            });
        }
        catch
        {
            // ignored
        }
    }

    public void StopAll()
    {
        if (_cancellationTokenSource is null)
        {
            return;
        }

        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = null;
    }

    public void Dispose()
    {
        StopAll();
        _cancellationTokenSource?.Dispose();
    }
}
