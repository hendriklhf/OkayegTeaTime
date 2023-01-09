using System.Collections;
using System.Collections.Generic;
using OkayegTeaTime.Database.Models;

namespace OkayegTeaTime.Database.Cache;

public abstract class DbCache<T> : IEnumerable<T> where T : CacheModel
{
    private protected readonly List<T> _items = new();
    private protected bool _containsAll;

    private protected abstract void GetAllItemsFromDatabase();

    public void Invalidate()
    {
        _items.Clear();
        _containsAll = false;
    }

    public IEnumerator<T> GetEnumerator()
    {
        GetAllItemsFromDatabase();
        return _items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
