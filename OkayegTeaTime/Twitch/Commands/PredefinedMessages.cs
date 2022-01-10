namespace OkayegTeaTime.Twitch.Commands;

public static class PredefinedMessages
{
    public const string ChannelEmotesError = "the channel doesn't have the specified amount of " +
        "emotes enabled or an error occurred";
    public const string NoModOrBroadcasterMessage = "you aren't a mod or the broadcaster";
    public const string TwitchUserDoesntExistMessage = "Twitch user doesn't exist";
    public const string TooManyRemindersMessage = "that person has too many reminders set for them";
    public const string UserNotFoundMessage = "could not find any matching user";
    public const string ReminderNotFoundMessage = "could not find any matching reminder";
}
