using MediatR;

using Telegram.Bot.Types;

namespace TelegramBirthdayAlarmBot.Commands
{
    internal record CompleteSetCongratulateLangCommand(
    string ChatId,
    User From,
    string Culture) : IRequest;
}
