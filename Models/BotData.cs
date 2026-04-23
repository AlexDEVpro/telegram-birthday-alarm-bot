namespace TelegramBirthdayAlarmBot.Models
{
    /// <summary>
    /// Root bot data object type.
    /// </summary>
    internal class BotData
    {
        /// <summary>
        /// A dictionary mapping chat IDs to chat data.
        /// </summary>
        public Dictionary<string, ChatData> Chats { get; set; } = new();
    }
}
