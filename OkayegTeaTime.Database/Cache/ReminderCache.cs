using System;
using System.Collections.Generic;
using System.Linq;
using HLE.Time;
using OkayegTeaTime.Database.Cache.Enums;
using OkayegTeaTime.Database.Models;

namespace OkayegTeaTime.Database.Cache;

public class ReminderCache : DbCache<Reminder>
{
    public Reminder? this[int id] => GetReminder(id);

    public int Add(Reminder reminder)
    {
        int id = DbController.AddReminder(new EntityFrameworkModels.Reminder(reminder));
        if (id == -1)
        {
            return -1;
        }

        reminder.Id = id;
        _items.Add(reminder);
        return id;
    }

    public int[] AddRange(IEnumerable<Reminder> reminders)
    {
        Reminder[] rmdrs = reminders.ToArray();
        int[] ids = DbController.AddReminders(rmdrs.Select(r => new EntityFrameworkModels.Reminder(r)));

        for (int i = 0; i < ids.Length; i++)
        {
            if (ids[i] == -1)
            {
                continue;
            }

            rmdrs[i].Id = ids[i];
            _items.Add(rmdrs[i]);
        }

        return ids;
    }

    public bool Remove(long userId, string username, int reminderId)
    {
        bool removed = DbController.RemoveReminder(userId, username, reminderId);
        if (!removed)
        {
            return false;
        }

        Reminder? reminder = this.FirstOrDefault(r => r.Id == reminderId);
        if (reminder is null)
        {
            return removed;
        }

        //wasn't sent, but basically equals deletion
        reminder.HasBeenSent = true;
        return true;
    }

    public IEnumerable<Reminder> GetRemindersFor(string username, ReminderType type = ReminderType.All)
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

        return this.Where(r => string.Equals(r.Target, username, StringComparison.OrdinalIgnoreCase) && EvaluateReminderType(r) && !r.HasBeenSent);
    }

    public IEnumerable<Reminder> GetExpiredReminders()
    {
        return this.Where(r => r.ToTime > 0 && r.ToTime <= TimeHelper.Now() && !r.HasBeenSent);
    }

    private Reminder? GetReminder(int id)
    {
        Reminder? reminder = this.FirstOrDefault(r => r.Id == id);
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
        if (_containsAll)
        {
            return;
        }

        EntityFrameworkModels.Reminder[] reminders = DbController.GetReminders();
        foreach (EntityFrameworkModels.Reminder rr in reminders)
        {
            if (_items.All(r => r.Id != rr.Id))
            {
                _items.Add(new(rr));
            }
        }

        _containsAll = true;
    }
}
