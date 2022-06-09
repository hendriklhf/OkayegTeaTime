using OkayegTeaTime.Database.EntityFrameworkModels;
using Timer = System.Timers.Timer;

namespace OkayegTeaTime.Database.Models;

public abstract class CacheModel
{
    private OkayegTeaTimeContext? _db;
    private readonly Timer _timer = new(1000);

    protected CacheModel()
    {
        _timer.Elapsed += (_, _) => _db?.SaveChanges();
    }

    ~CacheModel()
    {
        _db?.SaveChanges();
    }

    private protected void EditedProperty()
    {
        if (_timer.Enabled)
        {
            _timer.Stop();
        }
        _timer.Start();
    }

    private protected OkayegTeaTimeContext GetDbContext()
    {
        _db ??= new();
        return _db;
    }
}
