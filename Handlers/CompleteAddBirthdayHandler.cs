using MediatR;

using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

using TelegramBirthdayAlarmBot.Commands;
using TelegramBirthdayAlarmBot.Constants;
using TelegramBirthdayAlarmBot.Services;

namespace TelegramBirthdayAlarmBot.Handlers
{
    internal class CompleteAddBirthdayHandler : IRequestHandler<CompleteAddBirthdayCommand>
    {
        private readonly ITelegramBotClient _bot;
        private readonly PendingAddStateService _stateService;
        private readonly StorageService _storage;

        public CompleteAddBirthdayHandler(
            ITelegramBotClient bot,
            PendingAddStateService stateService,
            StorageService storage)
        {
            _bot = bot;
            _stateService = stateService;
            _storage = storage;
        }

        public async Task Handle(CompleteAddBirthdayCommand request, CancellationToken cancellationToken)
        {
            var chatId = request.ChatId;
            var from = request.From;
            var text = request.Text;

            if (!DateTime.TryParse(text, out var date))
            {
                await _bot.SendMessage(
                    chatId,
                    $"{Resources.BotMessages.InvalidDateWarning}{Environment.NewLine}{Resources.BotMessages.AddBirthdayStep2InvalidDateAppendix}",
                    ParseMode.Html,
                    replyMarkup: new ReplyKeyboardMarkup(new[]
                    {
                        new KeyboardButton[] { BotCommands.Cancel }
                    })
                    {
                        ResizeKeyboard = true,
                        OneTimeKeyboard = true
                    },
                    disableNotification: true
                );

                return;
            }

            var usernameOrFirstName = from.Username != null ? "@" + from.Username : from.FirstName;

            bool added = _storage.AddBirthday(chatId, from.Id, usernameOrFirstName, date);
            if (added)
            {
                await _bot.SendMessage(chatId,
                    string.Format(Resources.BotMessages.AddBirthdaySuccess, usernameOrFirstName),
                    disableNotification: true);

                _stateService.RemovePending(from.Id);
            }
            else
            {
                await _bot.SendMessage(chatId,
                    Resources.BotMessages.DuplicatedUserMessage,
                    disableNotification: true);
            }
        }
    }
}
