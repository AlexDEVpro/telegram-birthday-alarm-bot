using MediatR;
using Telegram.Bot.Types;

namespace TelegramBirthdayAlarmBot.Commands
{
    internal record ListCommand(string ChatId, User From) : IRequest;
}