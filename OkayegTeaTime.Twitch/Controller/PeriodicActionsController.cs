using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OkayegTeaTime.Database;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Controller;

public sealed class PeriodicActionsController : IDisposable
{
    private CancellationTokenSource _cancellationTokenSource = new();
    private readonly IEnumerable<PeriodicAction> _periodicActions;

    public PeriodicActionsController(IEnumerable<PeriodicAction> periodicActions)
    {
        _periodicActions = periodicActions;
    }

    ~PeriodicActionsController()
    {
        StopAll();
        _cancellationTokenSource.Dispose();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        StopAll();
        _cancellationTokenSource.Dispose();
    }

    public void StartAll()
    {
        Parallel.ForEachAsync(_periodicActions, _cancellationTokenSource.Token, async (periodicAction, cancellationToken) =>
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

    public void StopAll()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = new();
    }
}
