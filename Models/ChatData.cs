namespace TelegramBirthdayAlarmBot.Models
{
    /// <summary>
    /// Represents all user related data per chat.
    /// </summary>
    internal class ChatData
    {
        /// <summary>
        /// A dictionary mapping username/display name to user birthday state.
        /// </summary>
        public Dictionary<string, BirthdayState> Users { get; set; } = new();
    }
}
