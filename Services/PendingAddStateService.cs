using TelegramBirthdayAlarmBot.Models;

namespace TelegramBirthdayAlarmBot.Services
{
    /// <summary>
    /// Service managing the birthday input step state for interactive command flow.
    /// </summary>
    internal class PendingAddStateService
    {
        private readonly Dictionary<long, PendingAddState> _pendingAdds = new();

        // Start of date input waiting mode (step 1 of the addbirthday command).
        public void BeginPending(string chatId, long userId)
        {
            _pendingAdds[userId] = new PendingAddState { ChatId = chatId };
        }

        // Checks whether the specified user has an active pending state in the specified chat.
        public bool IsPending(string chatId, long userId)
        {
            return _pendingAdds.TryGetValue(userId, out var state) && state.ChatId == chatId;
        }

        // Remove the waiting state.
        public bool RemovePending(long userId)
        {
            return _pendingAdds.Remove(userId);
        }
    }
}
