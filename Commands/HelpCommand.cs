using MediatR;

using Telegram.Bot.Types;

namespace TelegramBirthdayAlarmBot.Commands
{
    internal record HelpCommand(string ChatId, User From) : IRequest;
}
