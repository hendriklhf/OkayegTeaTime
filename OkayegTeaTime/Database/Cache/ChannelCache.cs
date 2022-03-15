using OkayegTeaTime.Database.Models;

namespace OkayegTeaTime.Database.Cache;

public class ChannelCache : DbCache<Channel>
{
    public Channel? this[int id] => GetChannel(id);

    public Channel? this[string name] => GetChannel(name);

    public void Add(int id, string name)
    {
        Channel channel = new(id, name);
        DbController.AddChannel(id, name);
        _items.Add(channel);
    }

    public void Remove(int id)
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

    private Channel? GetChannel(int id)
    {
        Channel? channel = _items.FirstOrDefault(c => c.Id == id);
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
        Channel? c = _items.FirstOrDefault(c => string.Equals(name, c.Name, StringComparison.OrdinalIgnoreCase));
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
        List<EntityFrameworkModels.Channel> channels = DbController.GetChannels();
        channels.ForEach(ch =>
        {
            if (_items.All(c => c.Id != ch.Id))
            {
                _items.Add(new(ch));
            }
        });
        _containsAll = true;
    }

    public override IEnumerator<Channel> GetEnumerator()
    {
        if (_containsAll)
        {
            return _items.GetEnumerator();
        }

        GetAllFromDb();

        return _items.GetEnumerator();
    }
}
