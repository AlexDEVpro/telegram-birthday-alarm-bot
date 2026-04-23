using MediatR;
using Microsoft.Extensions.Options;
using System.Globalization;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using TelegramBirthdayAlarmBot.Commands;
using TelegramBirthdayAlarmBot.Configuration;
using TelegramBirthdayAlarmBot.Services;
using TelegramBirthdayAlarmBot.Services.Localization;

namespace TelegramBirthdayAlarmBot.Handlers
{
    internal class AddBirthdayHandler : IRequestHandler<AddBirthdayCommand>
    {
        private readonly ITelegramBotClient _bot;
        private readonly PendingAddStateService _stateService;
        private readonly UserCultureResolver _userCultureResolver;
        private readonly StorageService _storage;
        private readonly IOptions<BirthdayOptions> _options;

        private CultureInfo _culture;

        public AddBirthdayHandler(ITelegramBotClient bot, PendingAddStateService stateService, UserCultureResolver userCultureResolver, StorageService storage, IOptions<BirthdayOptions> options)
        {
            _bot = bot;
            _stateService = stateService;
            _userCultureResolver = userCultureResolver;
            _storage = storage;
            _options = options;

            _culture = new CultureInfo(options.Value.DateCulture);
        }

        public async Task Handle(AddBirthdayCommand request, CancellationToken cancellationToken)
        {
            var chatId = request.ChatId;
            var from = request.From;
            var text = request.Text;

            // Resolve user culture.
            _culture = _userCultureResolver.Resolve(from.LanguageCode);

            var parts = text.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);

            // Add birthday interactive mode. Step 1.
            if (parts.Length == 1)
            {
                _stateService.BeginPending(chatId, from.Id);

                await _bot.SendMessage(
                    chatId,
                    Resources.BotMessages.AddBirthdayStep1,
                    ParseMode.Html
                );

                return;
            }

            // If there are command arguments.
            var input = parts[1].Trim();

            long? userId;
            string usernameOrDisplayName;

            if (input.StartsWith("@"))
            {
                // Admin section.
                if (!_options.Value.AdminIDs.Contains(from.Id))
                {
                    await _bot.SendMessage(chatId, Resources.BotMessages.AddBirthdayOfOtherUserAdminOnly);

                    return;
                }

                var split = input.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length < 2)
                {
                    await _bot.SendMessage(chatId, $"{Resources.BotMessages.FormatPrefix} {Resources.BotMessages.AddBirthdayByUsernameFormat}", ParseMode.Html);
                    
                    return;
                }

                userId = null; // Other user's ID is unknown at the moment.
                usernameOrDisplayName = split[0];

                input = split[1];
            }
            else
            {
                userId = from.Id;
                usernameOrDisplayName = from.Username != null ? "@" + from.Username : from.FirstName;
            }

            if (!DateTime.TryParse(input, _culture, out var date))
            {
                await _bot.SendMessage(chatId, Resources.BotMessages.InvalidDateWarning);

                return;
            }

            bool added = _storage.AddBirthday(chatId, userId, usernameOrDisplayName, date);
            if (!added)
            {
                await _bot.SendMessage(chatId, Resources.BotMessages.DuplicatedUserMessage);

                return;
            }

            await _bot.SendMessage(chatId, string.Format(Resources.BotMessages.AddBirthdaySuccess, usernameOrDisplayName));
        }
    }
}
