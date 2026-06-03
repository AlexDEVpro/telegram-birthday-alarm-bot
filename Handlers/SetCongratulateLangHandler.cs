using MediatR;

using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

using TelegramBirthdayAlarmBot.Commands;
using TelegramBirthdayAlarmBot.Constants;
using TelegramBirthdayAlarmBot.Models;
using TelegramBirthdayAlarmBot.Services;

namespace TelegramBirthdayAlarmBot.Handlers
{
    internal class SetCongratulateLangHandler
    : IRequestHandler<SetCongratulateLangCommand>
    {
        private readonly ITelegramBotClient _bot;
        private readonly PendingSetCongratulateLangStateService _stateService;
        private readonly BotPermissionService _permissionService;

        public SetCongratulateLangHandler(
            ITelegramBotClient bot,
            PendingSetCongratulateLangStateService stateService,
            BotPermissionService permissionService)
        {
            _bot = bot;
            _stateService = stateService;
            _permissionService = permissionService;
        }

        public async Task Handle(
            SetCongratulateLangCommand request,
            CancellationToken cancellationToken)
        {
            var chatId = request.ChatId;
            var from = request.From;

            if (!await _permissionService.HasPermissionAsync(
                    chatId,
                    from.Id,
                    BotPermission.SetCongratulateLang))
            {
                await _bot.SendMessage(
                    chatId,
                    Resources.BotMessages.SetCongratulateLangOwnerOnly,
                    disableNotification: true);

                return;
            }

            _stateService.Begin(chatId, from.Id);

            await _bot.SendMessage(
                chatId,
                Resources.BotMessages.ChooseCongratulateLang,
                replyMarkup: new ReplyKeyboardMarkup([
                    SupportedLanguages.All
                        .Select(x => new KeyboardButton(x.Label))
                        .ToArray(),
                    [ new($"/{BotCommands.Cancel}") ]
                ])
                {
                    ResizeKeyboard = true,
                    OneTimeKeyboard = true
                },
                disableNotification: true);
        }
    }
}
