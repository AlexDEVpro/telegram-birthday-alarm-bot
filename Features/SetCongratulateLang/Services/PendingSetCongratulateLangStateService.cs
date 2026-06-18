using TelegramBirthdayAlarmBot.Features.SetCongratulateLang.Models;

namespace TelegramBirthdayAlarmBot.Features.SetCongratulateLang.Services
{
    /// <summary>
    /// Service managing the congratulate lang set step state for interactive command flow.
    /// </summary>
    internal class PendingSetCongratulateLangStateService
    {
        private readonly Dictionary<long, PendingSetCongratulateLangState> _states = new();

        public void Begin(long chatId, long userId)
        {
            _states[userId] = new PendingSetCongratulateLangState
            {
                ChatId = chatId
            };
        }

        public bool IsPending(long chatId, long userId)
        {
            return _states.TryGetValue(userId, out var state) && state.ChatId == chatId;
        }

        public bool Remove(long userId)
        {
            return _states.Remove(userId);
        }
    }
}
