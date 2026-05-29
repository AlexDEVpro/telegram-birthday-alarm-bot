using MediatR;

using Telegram.Bot;

using TelegramBirthdayAlarmBot.Commands;
using TelegramBirthdayAlarmBot.Services;

namespace TelegramBirthdayAlarmBot.Handlers
{
    internal class CancelHandler : IRequestHandler<CancelCommand>
    {
        private readonly ITelegramBotClient _bot;
        private readonly PendingAddStateService _stateService;

        public CancelHandler(ITelegramBotClient bot, PendingAddStateService stateService)
        {
            _bot = bot;
            _stateService = stateService;
        }

        public async Task Handle(CancelCommand request, CancellationToken cancellationToken)
        {
            var chatId = request.ChatId;
            var from = request.From;

            if (_stateService.RemovePending(from.Id))
            {
                await _bot.SendMessage(chatId,
                    Resources.BotMessages.ActionCancelled,
                    disableNotification: true);
            }
            else
            {
                await _bot.SendMessage(chatId,
                    Resources.BotMessages.NoActiveActionsToCancel,
                    disableNotification: true);
            }
        }
    }
}
