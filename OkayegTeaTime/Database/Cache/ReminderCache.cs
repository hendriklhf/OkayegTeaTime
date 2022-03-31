using OkayegTeaTime.Database.Cache.Enums;
using OkayegTeaTime.Database.Models;

namespace OkayegTeaTime.Database.Cache;

public class ReminderCache : DbCache<Reminder>
{
    public Reminder? this[int id] => GetReminder(id);

    public int? Add(string creator, string target, string message, string channel, long toTime = 0)
    {
        int? id = DbController.AddReminder(creator, target, message, channel, toTime);
        if (!id.HasValue)
        {
            return null;
        }

        Reminder reminder = new(id.Value, creator, target, message, channel, toTime);
        _items.Add(reminder);
        return id;
    }

    public int?[] AddRange(IEnumerable<(string Creator, string Target, string Message, string Channel, long ToTime)> values)
    {
        (string Creator, string Target, string Message, string Channel, long ToTime)[]? reminders = values.ToArray();
        int?[] ids = DbController.AddReminders(reminders);

        for (int i = 0; i < reminders.Length; i++)
        {
            if (!ids[i].HasValue)
            {
                continue;
            }

            Reminder r = new(ids[i]!.Value, reminders[i].Creator, reminders[i].Target, reminders[i].Message, reminders[i].Channel, reminders[i].ToTime);
            _items.Add(r);
        }

        return ids;
    }

    public bool Remove(int userId, string username, int reminderId)
    {
        bool removed = DbController.RemoveReminder(userId, username, reminderId);
        if (!removed)
        {
            return false;
        }

        Reminder? reminder = _items.FirstOrDefault(r => r.Id == reminderId);
        if (reminder is null)
        {
            return removed;
        }

        _items.Remove(reminder);
        return true;
    }

    public void RemoveRange(IEnumerable<int> reminderIds)
    {
        foreach (int id in reminderIds)
        {
            Reminder? r = this[id];
            if (r is null)
            {
                continue;
            }

            _items.Remove(r);
            DbController.RemoveReminder(r.Id);
        }
    }

    public Reminder[] GetReminderFor(string name, ReminderType type = ReminderType.All)
    {
        bool EvaluateReminderType(Reminder r)
        {
            return type switch
            {
                ReminderType.All => true,
                ReminderType.Timed => r.ToTime > 0,
                ReminderType.NonTimed => r.ToTime == 0,
                _ => false
            };
        }

        return _items.Where(r => string.Equals(r.Target, name, StringComparison.OrdinalIgnoreCase) && EvaluateReminderType(r)).ToArray();
    }

    private Reminder? GetReminder(int id)
    {
        Reminder? reminder = _items.FirstOrDefault(r => r.Id == id);
        if (reminder is not null)
        {
            return reminder;
        }

        EntityFrameworkModels.Reminder? efReminder = DbController.GetReminder(id);
        if (efReminder is null)
        {
            return null;
        }

        reminder = new(efReminder);
        _items.Add(reminder);
        return reminder;
    }

    private protected override void GetAllFromDb()
    {
        List<EntityFrameworkModels.Reminder> reminders = DbController.GetReminders();
        reminders.ForEach(rr =>
        {
            if (_items.All(r => r.Id != rr.Id))
            {
                _items.Add(new(rr));
            }
        });
        _containsAll = true;
    }

    public override IEnumerator<Reminder> GetEnumerator()
    {
        if (_containsAll)
        {
            return _items.GetEnumerator();
        }

        GetAllFromDb();

        return _items.GetEnumerator();
    }
}
