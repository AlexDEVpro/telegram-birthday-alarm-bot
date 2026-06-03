using MediatR;

using Telegram.Bot;
using Telegram.Bot.Types.Enums;

using TelegramBirthdayAlarmBot.Commands;
using TelegramBirthdayAlarmBot.Models;
using TelegramBirthdayAlarmBot.Services;

namespace TelegramBirthdayAlarmBot.Handlers
{
    internal class AddBirthdayHandler : IRequestHandler<AddBirthdayCommand>
    {
        private readonly ITelegramBotClient _bot;
        private readonly PendingAddStateService _stateService;
        private readonly BotPermissionService _botPermissionService;
        private readonly StorageService _storage;

        public AddBirthdayHandler(
            ITelegramBotClient bot,
            PendingAddStateService stateService,
            BotPermissionService botPermissionService,
            StorageService storage)
        {
            _bot = bot;
            _stateService = stateService;
            _botPermissionService = botPermissionService;
            _storage = storage;
        }

        public async Task Handle(AddBirthdayCommand request, CancellationToken cancellationToken)
        {
            var chatId = request.ChatId;
            var from = request.From;
            var text = request.Text;

            var parts = text.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);

            // Add birthday interactive mode. Step 1.
            if (parts.Length == 1)
            {
                _stateService.BeginPending(chatId, from.Id);

                await _bot.SendMessage(
                    chatId,
                    Resources.BotMessages.AddBirthdayStep1,
                    ParseMode.Html,
                    disableNotification: true
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
                if (!await _botPermissionService.HasPermissionAsync(chatId,
                    from.Id,
                    BotPermission.ManageOtherBirthdays))
                {
                    await _bot.SendMessage(chatId,
                        Resources.BotMessages.AddBirthdayOfOtherUserAdminOnly,
                        disableNotification: true);

                    return;
                }

                var split = input.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length < 2)
                {
                    await _bot.SendMessage(chatId,
                        $"{Resources.BotMessages.FormatPrefix} {Resources.BotMessages.AddBirthdayByUsernameFormat}",
                        ParseMode.Html,
                        disableNotification: true);

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

            if (!DateTime.TryParse(input, out var date))
            {
                await _bot.SendMessage(chatId,
                    Resources.BotMessages.InvalidDateWarning,
                    disableNotification: true);

                return;
            }

            bool added = _storage.AddBirthday(chatId, userId, usernameOrDisplayName, date);
            if (!added)
            {
                await _bot.SendMessage(chatId,
                    Resources.BotMessages.DuplicatedUserMessage,
                    disableNotification: true);

                return;
            }

            await _bot.SendMessage(chatId,
                string.Format(Resources.BotMessages.AddBirthdaySuccess, usernameOrDisplayName),
                disableNotification: true);
        }
    }
}
