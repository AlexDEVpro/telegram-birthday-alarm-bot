using MediatR;

using Telegram.Bot.Types;

namespace TelegramBirthdayAlarmBot.Features.Cancel.Commands
{
    internal record CancelCommand(
        long ChatId,
        User From) : IRequest;
}
