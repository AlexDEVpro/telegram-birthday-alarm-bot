using System.Text;

using MediatR;

using Telegram.Bot;
using Telegram.Bot.Types.Enums;

using TelegramBirthdayAlarmBot.Commands;

namespace TelegramBirthdayAlarmBot.Handlers
{
    internal class HelpHandler : IRequestHandler<HelpCommand>
    {
        private readonly ITelegramBotClient _bot;

        private readonly string _helpMessage;

        public HelpHandler(ITelegramBotClient bot)
        {
            _bot = bot;

            var sb = new StringBuilder();
            sb.AppendLine(Resources.BotMessages.HelpMessageTitle);
            sb.AppendLine();
            sb.AppendLine(Resources.BotMessages.AddbirthdayCommandHelp);
            sb.AppendLine();
            sb.AppendLine(Resources.BotMessages.RemovebirthdayCommandHelp);
            sb.AppendLine();
            sb.AppendLine(Resources.BotMessages.ListCommandHelp);
            sb.AppendLine();
            sb.Append(Resources.BotMessages.CancelCommandHelp);

            _helpMessage = sb.ToString();
        }

        public async Task Handle(HelpCommand request, CancellationToken cancellationToken)
        {
            var chatId = request.ChatId;

            await _bot.SendMessage(chatId,
                _helpMessage,
                ParseMode.Html,
                disableNotification: true);
        }
    }
}
