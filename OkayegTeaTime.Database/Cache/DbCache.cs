using System.Collections;
using System.Collections.Generic;
using OkayegTeaTime.Database.Models;

namespace OkayegTeaTime.Database.Cache;

public abstract class DbCache<T> : IEnumerable<T> where T : CacheModel
{
    private protected readonly Dictionary<long, T> _items = new();
    private protected bool _containsAll;

    protected DbCache()
    {
        GetAllItemsFromDatabase();
    }

    private protected abstract void GetAllItemsFromDatabase();

    public void Invalidate()
    {
        _items.Clear();
        _containsAll = false;
    }

    public IEnumerator<T> GetEnumerator()
    {
        GetAllItemsFromDatabase();
        return _items.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
