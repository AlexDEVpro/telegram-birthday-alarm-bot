using MediatR;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using TelegramBirthdayAlarmBot.Commands;
using TelegramBirthdayAlarmBot.Configuration;
using TelegramBirthdayAlarmBot.Services;
using TelegramBirthdayAlarmBot.Services.Localization;

namespace TelegramBirthdayAlarmBot.Handlers
{
    internal class ListHandler : IRequestHandler<ListCommand>
    {
        private readonly ITelegramBotClient _bot;
        private readonly UserCultureResolver _userCultureResolver;
        private readonly StorageService _storage;

        private CultureInfo _culture;

        public ListHandler(ITelegramBotClient bot, UserCultureResolver userCultureResolver, StorageService storage, IOptions<BirthdayOptions> options)
        {
            _bot = bot;
            _userCultureResolver = userCultureResolver;
            _storage = storage;

            _culture = new CultureInfo(options.Value.DateCulture);
        }

        public async Task Handle(ListCommand request, CancellationToken cancellationToken)
        {
            var chatId = request.ChatId;
            var from = request.From;

            // Resolve user culture.
            _culture = _userCultureResolver.Resolve(from.LanguageCode);

            var list = _storage.GetSortedBirthdays(chatId);
            if (list.Count == 0)
            {
                await _bot.SendMessage(chatId, Resources.BotMessages.ListEmpty);

                return;
            }

            var sb = new StringBuilder();
            sb.AppendLine(Resources.BotMessages.ListTitle);

            foreach (var bs in list)
            {
                sb.AppendLine();
                sb.Append($"{bs.DisplayName} — {bs.Date.ToString(Resources.BotMessages.ListItemDateFormat, _culture)}");
            }

            await _bot.SendMessage(chatId, sb.ToString(), ParseMode.Html);
        }
    }
}
