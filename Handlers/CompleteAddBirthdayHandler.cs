using System.Globalization;

using MediatR;

using Microsoft.Extensions.Options;

using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

using TelegramBirthdayAlarmBot.Commands;
using TelegramBirthdayAlarmBot.Configuration;
using TelegramBirthdayAlarmBot.Constants;
using TelegramBirthdayAlarmBot.Services;
using TelegramBirthdayAlarmBot.Services.Localization;

namespace TelegramBirthdayAlarmBot.Handlers
{
    internal class CompleteAddBirthdayHandler : IRequestHandler<CompleteAddBirthdayCommand>
    {
        private readonly ITelegramBotClient _bot;
        private readonly PendingAddStateService _stateService;
        private readonly UserCultureResolver _userCultureResolver;
        private readonly StorageService _storage;

        private CultureInfo _culture;

        public CompleteAddBirthdayHandler(ITelegramBotClient bot, PendingAddStateService stateService, UserCultureResolver userCultureResolver, StorageService storage, IOptions<BirthdayOptions> birthdayOptions)
        {
            _bot = bot;
            _stateService = stateService;
            _userCultureResolver = userCultureResolver;
            _storage = storage;

            _culture = new CultureInfo(birthdayOptions.Value.DateCulture);
        }

        public async Task Handle(CompleteAddBirthdayCommand request, CancellationToken cancellationToken)
        {
            var chatId = request.ChatId;
            var from = request.From;
            var text = request.Text;

            // Resolve user culture.
            _culture = _userCultureResolver.Resolve(from.LanguageCode);

            if (!DateTime.TryParse(text, _culture, out var date))
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
