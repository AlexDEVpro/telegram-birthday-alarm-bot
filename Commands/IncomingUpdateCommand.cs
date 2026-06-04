using MediatR;

using Telegram.Bot.Types;

namespace TelegramBirthdayAlarmBot.Commands
{
    internal record IncomingUpdateCommand(
        long ChatId,
        User From,
        string Text) : IRequest;
}
