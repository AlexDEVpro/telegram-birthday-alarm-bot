namespace TelegramBirthdayAlarmBot.Models
{
    /// <summary>
    /// Represents the state of a pending add birthday operation for a user.
    /// </summary>
    internal class PendingAddState
    {
        /// <summary>
        /// Chat ID where the add birthday operation is currently in progress.
        /// </summary>
        public long ChatId { get; set; }
    }
}
