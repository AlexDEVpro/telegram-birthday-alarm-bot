using MediatR;
using Telegram.Bot.Types;

namespace TelegramBirthdayAlarmBot.Commands
{
    internal record CancelCommand(string ChatId, User From) : IRequest;
}
