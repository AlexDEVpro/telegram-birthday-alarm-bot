namespace TelegramBirthdayAlarmBot.Features.SetCongratulateLang.Models
{
    /// <summary>
    /// Represents the state of a pending set congratulate lang operation for a user.
    /// </summary>
    internal class PendingSetCongratulateLangState
    {
        /// <summary>
        /// Chat ID where the set congratulate lang operation is currently in progress.
        /// </summary>
        public required long ChatId { get; init; }
    }
}
