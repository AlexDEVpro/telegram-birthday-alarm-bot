using System.Text;

using MediatR;

using Microsoft.Extensions.Options;

using Telegram.Bot;
using Telegram.Bot.Types.Enums;

using TelegramBirthdayAlarmBot.Configuration;
using TelegramBirthdayAlarmBot.Features.Help.Commands;
using TelegramBirthdayAlarmBot.Infrastructure.Authorization.Models;
using TelegramBirthdayAlarmBot.Infrastructure.Authorization.Services;

namespace TelegramBirthdayAlarmBot.Features.Help.Handlers
{
    internal class HelpHandler : IRequestHandler<HelpCommand>
    {
        private readonly ITelegramBotClient _bot;
        private readonly BotPermissionService _botPermissionService;

        public HelpHandler(ITelegramBotClient bot,
            BotPermissionService botPermissionService,
            IOptions<TelegramOptions> telegramOptions)
        {
            _bot = bot;
            _botPermissionService = botPermissionService;
        }

        public async Task Handle(HelpCommand request, CancellationToken cancellationToken)
        {
            var chatId = request.ChatId;
            var from = request.From;

            bool canManageOtherBirthdays = await _botPermissionService.HasPermissionAsync(chatId,
                        from.Id,
                        BotPermission.ManageOtherBirthdays);

            var sb = new StringBuilder();
            sb.AppendLine(Resources.BotMessages.HelpMessageTitle);
            sb.AppendLine();
            sb.AppendLine(Resources.BotMessages.AddbirthdayCommandHelp);
            sb.AppendLine();
            // Admin section.
            if (canManageOtherBirthdays)
            {
                sb.AppendLine(Resources.BotMessages.AddbirthdayOfOtherUserCommandHelp);
                sb.AppendLine();
            }
            sb.AppendLine(Resources.BotMessages.RemovebirthdayCommandHelp);
            sb.AppendLine();
            // Admin section.
            if (canManageOtherBirthdays)
            {
                sb.AppendLine(Resources.BotMessages.RemovebirthdayOfOtherUserCommandHelp);
                sb.AppendLine();
            }
            sb.AppendLine(Resources.BotMessages.ListCommandHelp);
            sb.AppendLine();
            sb.Append(Resources.BotMessages.CancelCommandHelp);

            await _bot.SendMessage(chatId,
                sb.ToString(),
                ParseMode.Html,
                disableNotification: true);
        }
    }
}
