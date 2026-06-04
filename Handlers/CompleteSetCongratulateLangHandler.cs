using MediatR;

using Telegram.Bot;

using TelegramBirthdayAlarmBot.Commands;
using TelegramBirthdayAlarmBot.Constants;
using TelegramBirthdayAlarmBot.Services;

namespace TelegramBirthdayAlarmBot.Handlers
{
    internal class CompleteSetCongratulateLangHandler
        : IRequestHandler<CompleteSetCongratulateLangCommand>
    {
        private readonly ITelegramBotClient _bot;
        private readonly StorageService _storage;
        private readonly PendingSetCongratulateLangStateService _pendingSetCongratulateLangStateService;

        public CompleteSetCongratulateLangHandler(
            ITelegramBotClient bot,
            StorageService storage,
            PendingSetCongratulateLangStateService pendingSetCongratulateLangStateService)
        {
            _bot = bot;
            _storage = storage;
            _pendingSetCongratulateLangStateService = pendingSetCongratulateLangStateService;
        }

        public async Task Handle(
            CompleteSetCongratulateLangCommand request,
            CancellationToken cancellationToken)
        {
            if (!_pendingSetCongratulateLangStateService.IsPending(request.ChatId,
                    request.From.Id))
            {
                return;
            }

            if (!SupportedLanguages.ByLabel.ContainsKey(request.LanguageLabel))
            {
                await _bot.SendMessage(
                    request.ChatId,
                    Resources.BotMessages.UnsupportedLanguage,
                    disableNotification: true);

                return;
            }

            _storage.SetCongratulateCulture(
                request.ChatId,
                request.LanguageLabel);

            _pendingSetCongratulateLangStateService.Remove(request.From.Id);

            await _bot.SendMessage(
                request.ChatId,
                string.Format(Resources.BotMessages.CongratulationsLanguageChanged, request.LanguageLabel),
                disableNotification: true);
        }
    }
}