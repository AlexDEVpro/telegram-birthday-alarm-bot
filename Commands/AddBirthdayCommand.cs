using MediatR;

using Telegram.Bot.Types;

namespace TelegramBirthdayAlarmBot.Commands
{
    internal record AddBirthdayCommand(
        long ChatId,
        User From,
        string Text) : IRequest;
}
