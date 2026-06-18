using MediatR;

using Telegram.Bot.Types;

namespace TelegramBirthdayAlarmBot.Features.SetCongratulateLang.Commands
{
    internal record SetCongratulateLangCommand(
        long ChatId,
        User From) : IRequest;
}
