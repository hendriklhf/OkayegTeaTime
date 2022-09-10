using System;
using System.Collections.Generic;
using System.Linq;
using HLE.Collections;
using HLE.Time;
using OkayegTeaTime.Database.Cache.Enums;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Files;

namespace OkayegTeaTime.Database.Cache;

public class ReminderCache : DbCache<Reminder>
{
    public Reminder? this[int id] => GetReminder(id);

    public int Add(Reminder reminder)
    {
        if (HasTooManyRemindersSet(reminder.Target, reminder.ToTime > 0))
        {
            return -1;
        }

        int id = DbController.AddReminder(new(reminder));
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
        int[] ids = new int[rmdrs.Length];
        for (int i = 0; i < rmdrs.Length; i++)
        {
            if (HasTooManyRemindersSet(rmdrs[i].Target, rmdrs[i].ToTime > 0))
            {
                ids[i] = -1;
                continue;
            }

            ids[i] = DbController.AddReminder(new(rmdrs[i]));
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

        //wasn't sent, but basically equals deletion from the memory cache
        reminder.HasBeenSent = true;
        return true;
    }

    /// <summary>
    /// Removes a reminder without checking for permission
    /// </summary>
    public void Remove(int reminderId)
    {
        DbController.RemoveReminder(reminderId);
        Reminder? reminder = this.FirstOrDefault(r => r.Id == reminderId);
        if (reminder is null)
        {
            return;
        }

        //wasn't sent, but basically equals deletion from the memory cache
        reminder.HasBeenSent = true;
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

    private bool HasTooManyRemindersSet(string target, bool isTimedReminder)
    {
        Func<Reminder, bool> condition = isTimedReminder switch
        {
            true => r => r.Target == target && r.ToTime == 0,
            _ => r => r.Target == target && r.ToTime > 0
        };

        return this.Count(condition) >= AppSettings.MaxReminders;
    }

    private protected override void GetAllFromDb()
    {
        if (_containsAll)
        {
            return;
        }

        DbController.GetReminders().Where(r => _items.All(i => i.Id != r.Id)).ForEach(r => _items.Add(new(r)));

        _containsAll = true;
    }
}