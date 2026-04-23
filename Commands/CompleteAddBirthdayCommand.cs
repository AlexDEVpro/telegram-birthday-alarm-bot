using MediatR;
using Telegram.Bot.Types;

namespace TelegramBirthdayAlarmBot.Commands
{
    internal record CompleteAddBirthdayCommand(string ChatId, User From, string Text) : IRequest;
}
