using MediatR;

namespace TelegramBirthdayAlarmBot.Commands
{
    internal record HelpCommand(string ChatId) : IRequest;
}
