using System;
using System.Threading;
using OkayegTeaTime.Database.EntityFrameworkModels;
using Timer = System.Timers.Timer;

namespace OkayegTeaTime.Database.Models;

#pragma warning disable CA1001
public abstract class CacheModel
#pragma warning restore CA1001
{
    private OkayegTeaTimeContext? _db;
    private readonly Timer _timer = new(1000);
    private protected readonly Mutex _mutex = new();

    protected CacheModel()
    {
        _timer.Elapsed += async (_, _) =>
        {
            OkayegTeaTimeContext db = GetContext();
            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                await DbController.LogExceptionAsync(ex);
            }
            finally
            {
                ReturnContext();
            }
        };
    }

    private protected OkayegTeaTimeContext GetContext()
    {
        _mutex.WaitOne();
        return _db ??= new();
    }

    private protected void ReturnContext()
    {
        _mutex.ReleaseMutex();
    }

    private protected void EditedProperty()
    {
        if (_timer.Enabled)
        {
            _timer.Stop();
        }

        _timer.Start();
    }
}
