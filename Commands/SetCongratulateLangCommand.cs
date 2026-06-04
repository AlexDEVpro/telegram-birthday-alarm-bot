using MediatR;

using Telegram.Bot.Types;

namespace TelegramBirthdayAlarmBot.Commands
{
    internal record SetCongratulateLangCommand(
        long ChatId,
        User From) : IRequest;
}
