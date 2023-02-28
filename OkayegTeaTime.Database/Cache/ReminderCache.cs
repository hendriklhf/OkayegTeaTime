using System;
using System.Linq;
using HLE.Collections;
using OkayegTeaTime.Database.Cache.Enums;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Settings;

namespace OkayegTeaTime.Database.Cache;

public sealed class ReminderCache : DbCache<Reminder>
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

    public int[] AddRange(Reminder[] reminders)
    {
        int[] ids = new int[reminders.Length];
        for (int i = 0; i < reminders.Length; i++)
        {
            if (HasTooManyRemindersSet(reminders[i].Target, reminders[i].ToTime > 0))
            {
                ids[i] = -1;
                continue;
            }

            ids[i] = DbController.AddReminder(new(reminders[i]));
            if (ids[i] == -1)
            {
                continue;
            }

            reminders[i].Id = ids[i];
            _items.Add(reminders[i]);
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

        Reminder? reminder = this[reminderId];
        if (reminder is null)
        {
            return removed;
        }

        //wasn't sent, but basically equals deletion from the memory cache
        reminder.HasBeenSent = true;
        return true;
    }

    /// <summary>
    ///     Removes a reminder without checking for permission
    /// </summary>
    public void Remove(int reminderId)
    {
        DbController.RemoveReminder(reminderId);
        Reminder? reminder = this[reminderId];
        if (reminder is null)
        {
            return;
        }

        //wasn't sent, but basically equals deletion from the memory cache
        reminder.HasBeenSent = true;
    }

    public Reminder[] GetRemindersFor(string username, ReminderType type)
    {
        static bool EvaluateReminderType(Reminder r, ReminderType t)
        {
            if ((int)(t & (ReminderType.Timed | ReminderType.NonTimed)) == (int)(ReminderType.Timed | ReminderType.NonTimed))
            {
                return true;
            }

            if ((int)(t & ReminderType.Timed) == (int)ReminderType.Timed)
            {
                return r.ToTime > 0;
            }

            if ((int)(t & ReminderType.NonTimed) == (int)ReminderType.NonTimed)
            {
                return r.ToTime == 0;
            }

            return false;
        }

        return this.Where(r => !r.HasBeenSent && username == r.Target && EvaluateReminderType(r, type)).ToArray();
    }

    public Reminder[] GetExpiredReminders()
    {
        return this.Where(r => r.ToTime > 0 && r.ToTime <= DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() && !r.HasBeenSent).ToArray();
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

    private protected override void GetAllItemsFromDatabase()
    {
        if (_containsAll)
        {
            return;
        }

        DbController.GetReminders().Where(r => _items.All(i => i.Id != r.Id)).ForEach(r => _items.Add(new(r)));
        _containsAll = true;
    }
}
