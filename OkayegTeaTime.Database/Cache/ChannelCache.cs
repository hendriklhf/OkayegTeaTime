using System;
using System.Linq;
using HLE.Collections;
using OkayegTeaTime.Database.Models;

namespace OkayegTeaTime.Database.Cache;

public class ChannelCache : DbCache<Channel>
{
    public Channel? this[long id] => GetChannel(id);

    public Channel? this[string name] => GetChannel(name);

    public void Add(long id, string name)
    {
        Channel channel = new(id, name);
        DbController.AddChannel(id, name);
        _items.Add(channel);
    }

    public void Remove(long id)
    {
        Channel? channel = this[id];
        if (channel is null)
        {
            return;
        }

        _items.Remove(channel);
        DbController.RemoveChannel(id);
    }

    public void Remove(string name)
    {
        Channel? channel = this[name];
        if (channel is null)
        {
            return;
        }

        _items.Remove(channel);
        DbController.RemoveChannel(name);
    }

    private Channel? GetChannel(long id)
    {
        Channel? channel = this.FirstOrDefault(c => c.Id == id);
        if (channel is not null)
        {
            return channel;
        }

        EntityFrameworkModels.Channel? efChannel = DbController.GetChannel(id);
        if (efChannel is null)
        {
            return null;
        }

        channel = new(efChannel);
        _items.Add(channel);
        return channel;
    }

    private Channel? GetChannel(string name)
    {
        Channel? c = this.FirstOrDefault(c => string.Equals(name, c.Name, StringComparison.OrdinalIgnoreCase));
        if (c is not null)
        {
            return c;
        }

        EntityFrameworkModels.Channel? efChannel = DbController.GetChannel(name);
        if (efChannel is null)
        {
            return null;
        }

        c = new(efChannel);
        _items.Add(c);
        return c;
    }

    private protected override void GetAllFromDb()
    {
        if (_containsAll)
        {
            return;
        }

        DbController.GetChannels().Where(c => !_items.All(i => c.Id != i.Id)).ForEach(c => _items.Add(new(c)));
        _containsAll = true;
    }
}
