namespace TelegramBirthdayAlarmBot.Infrastructure.Persistence.Models;

/// <summary>
/// User's birthday celebration info.
/// </summary>
internal class BirthdayState
{
    /// <summary>
    /// The user ID. May initially be null, as it can be retrieved either from a reply to the user or from the user's messages in the chat.
    /// </summary>
    public long? UserId { get; set; }

    /// <summary>
    /// User @username, can be hidden.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// User display name.
    /// </summary>
    public string DisplayName { get; set; } = "";

    /// <summary>
    /// User birthday date.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Last celebrated year for duplicated congratulations prevention.
    /// </summary>
    public int LastCelebratedYear { get; set; }
}
