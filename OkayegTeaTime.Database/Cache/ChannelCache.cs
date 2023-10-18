using System;
using System.Linq;
using HLE.Collections;
using OkayegTeaTime.Database.Models;

namespace OkayegTeaTime.Database.Cache;

public sealed class ChannelCache : DbCache<Channel>
{
    public Channel? this[long id] => Get(id);

    public Channel? this[string name] => Get(name);

    public void Add(long id, string name)
    {
        Channel channel = new(id, name);
        DbController.AddChannel(id, name);
        _items.AddOrSet(id, channel);
    }

    public void Remove(long id)
    {
        _items.TryRemove(id, out _);
        DbController.RemoveChannel(id);
    }

    public void Remove(string name)
    {
        Channel? channel = this[name];
        if (channel is null)
        {
            return;
        }

        _items.TryRemove(channel.Id, out _);
        DbController.RemoveChannel(channel.Id);
    }

    private Channel? Get(long id)
    {
        _items.TryGetValue(id, out Channel? channel);
        return channel;
    }

    private Channel? Get(string name) => this.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

    private protected override void GetAllItemsFromDatabase()
    {
        if (_containsAll)
        {
            return;
        }

        foreach (EntityFrameworkModels.Channel channel in DbController.GetChannels().Where(c => _items.All(i => c.Id != i.Value.Id)))
        {
            _items.AddOrSet(channel.Id, new(channel));
        }

        _containsAll = true;
    }
}
