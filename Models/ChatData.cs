using TelegramBirthdayAlarmBot.Constants;

namespace TelegramBirthdayAlarmBot.Models
{
    /// <summary>
    /// Represents all user related data per chat.
    /// </summary>
    internal class ChatData
    {
        /// <summary>
        /// Congratulate message culture. This is used to determine the language of the birthday congratulation message.
        /// </summary>
        public string CongratulateCulture { get; set; } = SupportedLanguages.Default.Culture;

        /// <summary>
        /// A dictionary mapping username/display name to user birthday state.
        /// </summary>
        public Dictionary<string, BirthdayState> Users { get; set; } = new();
    }
}
