using System;
using System.Linq;
using HLE.Collections;
using OkayegTeaTime.Database.Cache.Enums;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Configuration;

namespace OkayegTeaTime.Database.Cache;

public sealed class ReminderCache : DbCache<Reminder>
{
    public Reminder? this[int id] => Get(id);

    public int Add(Reminder reminder)
    {
        if (HasTooManyRemindersSet(reminder.Target, reminder.ToTime != 0))
        {
            return -1;
        }

        int id = DbController.AddReminder(new(reminder));
        if (id == -1)
        {
            return -1;
        }

        reminder.Id = id;
        _items.AddOrSet(id, reminder);
        return id;
    }

    public int[] AddRange(Reminder[] reminders)
    {
        int[] ids = new int[reminders.Length];
        for (int i = 0; i < reminders.Length; i++)
        {
            if (HasTooManyRemindersSet(reminders[i].Target, reminders[i].ToTime != 0))
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
            _items.AddOrSet(ids[i], reminders[i]);
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
            return false;
        }

        reminder.HasBeenSent = true;
        return true;
    }

    /// <summary>
    /// Removes a reminder without checking for permission
    /// </summary>
    /// <param name="reminderId">The id of the reminder.</param>
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

    public Reminder[] GetReminders(string username, ReminderTypes types)
    {
        using PooledList<Reminder> reminders = [];
        foreach (Reminder reminder in _items.Values)
        {
            if (!reminder.HasBeenSent && username == reminder.Target && EvaluateReminderTypes(reminder, types))
            {
                reminders.Add(reminder);
            }
        }

        return reminders.ToArray();

        static bool EvaluateReminderTypes(Reminder reminder, ReminderTypes types)
        {
            if ((types & (ReminderTypes.Timed | ReminderTypes.NonTimed)) == (ReminderTypes.Timed | ReminderTypes.NonTimed))
            {
                return true;
            }

            if ((types & ReminderTypes.Timed) != 0)
            {
                return reminder.ToTime != 0;
            }

            if ((types & ReminderTypes.NonTimed) != 0)
            {
                return reminder.ToTime == 0;
            }

            return false;
        }
    }

    public Reminder[] GetExpiredReminders()
    {
#pragma warning disable S6354
        long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
#pragma warning restore S6354

        using PooledList<Reminder> reminders = [];
        foreach (Reminder reminder in _items.Values)
        {
            if (reminder.ToTime != 0 && !reminder.HasBeenSent && reminder.ToTime <= now)
            {
                reminders.Add(reminder);
            }
        }

        return reminders.ToArray();
    }

    private Reminder? Get(int id)
    {
        _items.TryGetValue(id, out Reminder? reminder);
        return reminder;
    }

    private bool HasTooManyRemindersSet(string target, bool isTimedReminder)
    {
        Func<Reminder, bool> condition = isTimedReminder switch
        {
            true => r => r.Target == target && r.ToTime == 0,
            _ => r => r.Target == target && r.ToTime != 0
        };

        return this.Count(condition) >= GlobalSettings.MaxReminders;
    }

    private protected override void GetAllItemsFromDatabase()
    {
        if (_containsAll)
        {
            return;
        }

        foreach (EntityFrameworkModels.Reminder reminder in DbController.GetReminders().Where(r => _items.All(i => i.Value.Id != r.Id)))
        {
            _items.AddOrSet(reminder.Id, new(reminder));
        }

        _containsAll = true;
    }
}
