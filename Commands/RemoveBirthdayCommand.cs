using MediatR;
using Telegram.Bot.Types;

namespace TelegramBirthdayAlarmBot.Commands
{
    internal record RemoveBirthdayCommand(string ChatId, User From, string Text) : IRequest;
}
