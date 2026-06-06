using MediatR;

using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

using TelegramBirthdayAlarmBot.Commands;
using TelegramBirthdayAlarmBot.Services;

namespace TelegramBirthdayAlarmBot.Handlers
{
    internal class CancelHandler : IRequestHandler<CancelCommand>
    {
        private readonly ITelegramBotClient _bot;
        private readonly PendingAddBirthdayStateService _pendingAddBirthdayStateService;

        public CancelHandler(
            ITelegramBotClient bot,
            PendingAddBirthdayStateService pendingAddBirthdayStateService)
        {
            _bot = bot;
            _pendingAddBirthdayStateService = pendingAddBirthdayStateService;
        }

        public async Task Handle(CancelCommand request, CancellationToken cancellationToken)
        {
            var chatId = request.ChatId;
            var from = request.From;

            if (_pendingAddBirthdayStateService.RemovePending(from.Id))
            {
                await _bot.SendMessage(chatId,
                    Resources.BotMessages.ActionCancelled,
                    replyMarkup: new ReplyKeyboardRemove(),
                    disableNotification: true);
            }
            else
            {
                await _bot.SendMessage(chatId,
                    Resources.BotMessages.NoActiveActionsToCancel,
                    replyMarkup: new ReplyKeyboardRemove(),
                    disableNotification: true);
            }
        }
    }
}
