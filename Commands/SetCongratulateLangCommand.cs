using MediatR;

using Telegram.Bot.Types;

namespace TelegramBirthdayAlarmBot.Commands
{
    internal record SetCongratulateLangCommand(
        string ChatId,
        User From) : IRequest;
}
