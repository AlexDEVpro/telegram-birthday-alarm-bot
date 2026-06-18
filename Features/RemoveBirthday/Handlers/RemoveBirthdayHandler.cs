using MediatR;

using Telegram.Bot;
using Telegram.Bot.Types.Enums;

using TelegramBirthdayAlarmBot.Features.RemoveBirthday.Commands;
using TelegramBirthdayAlarmBot.Infrastructure.Authorization.Models;
using TelegramBirthdayAlarmBot.Infrastructure.Authorization.Services;
using TelegramBirthdayAlarmBot.Infrastructure.Persistence.Services;

namespace TelegramBirthdayAlarmBot.Features.RemoveBirthday.Handlers
{
    internal class RemoveBirthdayHandler : IRequestHandler<RemoveBirthdayCommand>
    {
        private readonly ITelegramBotClient _bot;
        private readonly BotPermissionService _botPermissionService;
        private readonly StorageService _storage;

        public RemoveBirthdayHandler(
            ITelegramBotClient bot,
            BotPermissionService botPermissionService,
            StorageService storage)
        {
            _bot = bot;
            _botPermissionService = botPermissionService;
            _storage = storage;
        }

        public async Task Handle(RemoveBirthdayCommand request, CancellationToken cancellationToken)
        {
            var chatId = request.ChatId;
            var from = request.From;
            var text = request.Text;

            var parts = text.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);

            long? userId;
            string usernameOrDisplayName;

            if (parts.Length == 1)
            {
                userId = from.Id;
                usernameOrDisplayName = from.Username != null ? "@" + from.Username : from.FirstName;
            }
            else
            {
                var input = parts[1].Trim();
                if (input.StartsWith("@"))
                {
                    // Admin section.
                    if (!await _botPermissionService.HasPermissionAsync(chatId,
                        from.Id,
                        BotPermission.ManageOtherBirthdays))
                    {
                        await _bot.SendMessage(chatId,
                            Resources.BotMessages.RemoveBirthdayOfOtherUserAdminOnly,
                            disableNotification: true);

                        return;
                    }

                    userId = null; // Other user's ID is unknown at the moment.
                    usernameOrDisplayName = input;
                }
                else
                {
                    await _bot.SendMessage(chatId,
                        $"{Resources.BotMessages.FormatPrefix}{Resources.BotMessages.RemoveBirthdayByUsernameFormat}",
                        ParseMode.Html,
                        disableNotification: true);

                    return;
                }
            }

            bool removed;
            if (userId != null)
                removed = _storage.RemoveBirthday(chatId, userId.Value);
            else
                removed = _storage.RemoveBirthday(chatId, usernameOrDisplayName);

            if (removed)
                await _bot.SendMessage(chatId,
                    string.Format(Resources.BotMessages.RemoveBirthdaySuccess, usernameOrDisplayName),
                    disableNotification: true);
            else
                await _bot.SendMessage(chatId,
                    Resources.BotMessages.BirthdayNotFound,
                    disableNotification: true);
        }
    }
}
