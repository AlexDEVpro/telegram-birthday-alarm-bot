using MediatR;

using Telegram.Bot.Types;

namespace TelegramBirthdayAlarmBot.Commands
{
    internal record HelpCommand(
        long ChatId,
        User From) : IRequest;
}
