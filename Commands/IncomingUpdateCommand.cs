using MediatR;
using Telegram.Bot.Types;

namespace TelegramBirthdayAlarmBot.Commands
{
    internal record IncomingUpdateCommand(string ChatId, User From, string Text) : IRequest;
}
