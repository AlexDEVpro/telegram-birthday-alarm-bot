using MediatR;

using Telegram.Bot.Types;

namespace TelegramBirthdayAlarmBot.Infrastructure.Telegram.Commands
{
    internal record IncomingUpdateCommand(
        long ChatId,
        User From,
        string Text) : IRequest;
}
