using MediatR;

using Telegram.Bot.Types;

namespace TelegramBirthdayAlarmBot.Commands
{
    internal record RemoveBirthdayCommand(
        long ChatId,
        User From,
        string Text) : IRequest;
}
