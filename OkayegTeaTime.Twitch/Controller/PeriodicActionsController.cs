using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OkayegTeaTime.Database;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Controller;

public sealed class PeriodicActionsController(IEnumerable<PeriodicAction> periodicActions) : IDisposable
{
    private readonly IEnumerable<PeriodicAction> _periodicActions = periodicActions;
    private CancellationTokenSource _cancellationTokenSource = new();

    public void Dispose()
    {
        StopAll();
        _cancellationTokenSource.Dispose();
    }

    public void StartAll()
    {
        try
        {
            Parallel.ForEachAsync(_periodicActions, _cancellationTokenSource.Token, static async (periodicAction, cancellationToken) =>
            {
                try
                {
                    PeriodicTimer timer = new(periodicAction.Interval);
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
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = new();
    }
}
