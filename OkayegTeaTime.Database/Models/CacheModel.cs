using System;
using System.Threading;
using OkayegTeaTime.Database.EntityFrameworkModels;
using Timer = System.Timers.Timer;

namespace OkayegTeaTime.Database.Models;

public abstract class CacheModel
{
    private protected OkayegTeaTimeContext DbContext => _db ??= new();

    private OkayegTeaTimeContext? _db;
    private readonly Timer _timer = new(1000);
    private protected readonly Mutex _mutex = new();

    protected CacheModel()
    {
        _timer.Elapsed += (_, _) =>
        {
            try
            {
                _mutex.WaitOne();
                _db?.SaveChanges();
                _mutex.ReleaseMutex();
            }
            catch (Exception ex)
            {
                DbController.LogException(ex);
            }
        };
    }

    ~CacheModel()
    {
        try
        {
            _mutex.WaitOne();
            _db?.SaveChanges();
            _mutex.ReleaseMutex();
            _db?.Dispose();
            _mutex.Dispose();
        }
        catch (Exception ex)
        {
            DbController.LogException(ex);
        }
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
