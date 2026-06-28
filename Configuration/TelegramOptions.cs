namespace TelegramBirthdayAlarmBot.Configuration;

internal class TelegramOptions
{
    /// <summary>
    /// Telegram bot token.
    /// </summary>
    public string? BotToken { get; set; }

    /// <summary>
    /// Telegram user IDs that have access to administrator commands
    /// regardless of their role in a Telegram group.
    ///
    /// This is necessary because administrator roles are sometimes used
    /// without actual administrative privileges for tagging purposes
    /// (for example, to indicate a user's nickname on another platform).
    ///
    /// Effectively, these users are super administrators for the entire bot instance.
    /// 
    /// TODO: Make AdminIDs per chat and allow the group owner to add and remove admins via @username or Telegram user ID.
    /// </summary>
    public long[] AdminIDs { get; set; } = [];
}
