using System.Text;

using MediatR;

using Telegram.Bot;
using Telegram.Bot.Types.Enums;

using TelegramBirthdayAlarmBot.Features.ListBirthdays.Commands;
using TelegramBirthdayAlarmBot.Infrastructure.Persistence.Services;

namespace TelegramBirthdayAlarmBot.Features.ListBirthdays.Handlers
{
    internal class ListHandler : IRequestHandler<ListCommand>
    {
        private readonly ITelegramBotClient _bot;
        private readonly StorageService _storage;

        public ListHandler(
            ITelegramBotClient bot,
            StorageService storage)
        {
            _bot = bot;
            _storage = storage;
        }

        public async Task Handle(ListCommand request, CancellationToken cancellationToken)
        {
            var chatId = request.ChatId;
            var from = request.From;

            var list = _storage.GetSortedBirthdays(chatId);
            if (list.Count == 0)
            {
                await _bot.SendMessage(chatId,
                    Resources.BotMessages.ListEmpty,
                    disableNotification: true);

                return;
            }

            var sb = new StringBuilder();
            sb.AppendLine(Resources.BotMessages.ListTitle);

            foreach (var bs in list)
            {
                sb.AppendLine();
                sb.Append($"{bs.DisplayName} — {bs.Date.ToString(Resources.BotMessages.ListItemDateFormat)}");
            }

            await _bot.SendMessage(chatId,
                sb.ToString(),
                ParseMode.Html,
                disableNotification: true);
        }
    }
}
