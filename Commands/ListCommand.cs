using MediatR;

using Telegram.Bot.Types;

namespace TelegramBirthdayAlarmBot.Commands
{
    internal record ListCommand(
        long ChatId,
        User From) : IRequest;
}