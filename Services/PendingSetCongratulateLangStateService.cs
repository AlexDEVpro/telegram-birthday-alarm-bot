using TelegramBirthdayAlarmBot.Models;

namespace TelegramBirthdayAlarmBot.Services
{
    /// <summary>
    /// Service managing the congratulate lang set step state for interactive command flow.
    /// </summary>
    internal class PendingSetCongratulateLangStateService
    {
        private readonly Dictionary<long, PendingSetCongratulateLangState> _states = new();

        public void Begin(string chatId, long userId)
        {
            _states[userId] = new PendingSetCongratulateLangState
            {
                ChatId = chatId
            };
        }

        public bool IsPending(string chatId, long userId)
        {
            return _states.TryGetValue(userId, out var state) && state.ChatId == chatId;
        }

        public bool Remove(long userId)
        {
            return _states.Remove(userId);
        }
    }
}
