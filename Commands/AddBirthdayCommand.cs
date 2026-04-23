using MediatR;
using Telegram.Bot.Types;

namespace TelegramBirthdayAlarmBot.Commands
{
    internal record AddBirthdayCommand(string ChatId, User From, string Text) : IRequest;
}
