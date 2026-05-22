using MediatR;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using TelegramBirthdayAlarmBot.Commands;
using TelegramBirthdayAlarmBot.Configuration;
using TelegramBirthdayAlarmBot.Services;

namespace TelegramBirthdayAlarmBot.Handlers
{
    internal class RemoveBirthdayHandler : IRequestHandler<RemoveBirthdayCommand>
    {
        private readonly ITelegramBotClient _bot;
        private readonly StorageService _storage;

        private long[] _adminIDs;

        public RemoveBirthdayHandler(ITelegramBotClient bot, StorageService storage, IOptions<TelegramOptions> telegramOptions)
        {
            _bot = bot;
            _storage = storage;

            _adminIDs = telegramOptions.Value.AdminIDs;
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
                    if (!_adminIDs.Contains(from.Id))
                    {
                        await _bot.SendMessage(chatId, Resources.BotMessages.RemoveBirthdayOfOtherUserAdminOnly);
                        
                        return;
                    }

                    userId = null; // Other user's ID is unknown at the moment.
                    usernameOrDisplayName = input;
                }
                else
                {
                    await _bot.SendMessage(chatId, $"{Resources.BotMessages.FormatPrefix}{Resources.BotMessages.RemoveBirthdayByUsernameFormat}", ParseMode.Html);
                    
                    return;
                }
            }

            bool removed;
            if (userId != null)
                removed = _storage.RemoveBirthday(chatId, userId.Value);
            else
                removed = _storage.RemoveBirthday(chatId, usernameOrDisplayName);

            if (removed)
                await _bot.SendMessage(chatId, string.Format(Resources.BotMessages.RemoveBirthdaySuccess, usernameOrDisplayName));
            else
                await _bot.SendMessage(chatId, Resources.BotMessages.BirthdayNotFound);
        }
    }
}
