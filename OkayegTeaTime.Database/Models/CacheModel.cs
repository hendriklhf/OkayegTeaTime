using System;
using OkayegTeaTime.Database.EntityFrameworkModels;
using Timer = System.Timers.Timer;

namespace OkayegTeaTime.Database.Models;

public abstract class CacheModel
{
    private protected OkayegTeaTimeContext DbContext
    {
        get
        {
            _db ??= new();
            return _db;
        }
    }

    private OkayegTeaTimeContext? _db;
    private readonly Timer _timer = new(1000);

    protected CacheModel()
    {
        _timer.Elapsed += (_, _) =>
        {
            try
            {
                _db?.SaveChanges();
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
            _db?.SaveChanges();
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
