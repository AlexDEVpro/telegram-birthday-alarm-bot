using MediatR;

using Telegram.Bot.Types;

namespace TelegramBirthdayAlarmBot.Commands
{
    internal record CancelCommand(
        long ChatId,
        User From) : IRequest;
}
