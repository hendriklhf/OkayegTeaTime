using System;
using System.Linq;
using System.Runtime.InteropServices;
using HLE.Collections;
using OkayegTeaTime.Database.Models;

namespace OkayegTeaTime.Database.Cache;

public sealed class ChannelCache : DbCache<Channel>
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
        GetAllItemsFromDatabase();
        Channel? channel = null;
        Span<Channel> channels = CollectionsMarshal.AsSpan(_items);
        int channelsLength = channels.Length;
        for (int i = 0; i < channelsLength; i++)
        {
            Channel c = channels[i];
            if (c.Id == id)
            {
                channel = c;
            }
        }

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
        GetAllItemsFromDatabase();
        Channel? channel = null;
        Span<Channel> channels = CollectionsMarshal.AsSpan(_items);
        int channelsLength = channels.Length;
        for (int i = 0; i < channelsLength; i++)
        {
            Channel c = channels[i];
            if (string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase))
            {
                channel = c;
            }
        }

        if (channel is not null)
        {
            return channel;
        }

        EntityFrameworkModels.Channel? efChannel = DbController.GetChannel(name);
        if (efChannel is null)
        {
            return null;
        }

        channel = new(efChannel);
        _items.Add(channel);
        return channel;
    }

    private protected override void GetAllItemsFromDatabase()
    {
        if (_containsAll)
        {
            return;
        }

        DbController.GetChannels().Where(c => _items.All(i => c.Id != i.Id)).ForEach(c => _items.Add(new(c)));
        _containsAll = true;
    }
}
