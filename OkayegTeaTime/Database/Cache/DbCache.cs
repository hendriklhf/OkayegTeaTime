﻿using System.Collections;
using OkayegTeaTime.Database.Models;

namespace OkayegTeaTime.Database.Cache;

public abstract class DbCache<T> : IEnumerable<T> where T : CacheModel
{
    private protected readonly List<T> _items = new();
    private protected bool _containsAll;

    private protected abstract void GetAllFromDb();

    public abstract IEnumerator<T> GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
