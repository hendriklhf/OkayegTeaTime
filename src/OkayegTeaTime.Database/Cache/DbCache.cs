using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using OkayegTeaTime.Database.Models;

namespace OkayegTeaTime.Database.Cache;

public abstract class DbCache<T> : IEnumerable<T> where T : CacheModel
{
    private protected readonly ConcurrentDictionary<long, T> _items = new();
    private protected bool _containsAll;

    // ReSharper disable once VirtualMemberCallInConstructor
#pragma warning disable CA2214, S1699
    protected DbCache() => GetAllItemsFromDatabase();
#pragma warning restore S1699, CA2214

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

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
