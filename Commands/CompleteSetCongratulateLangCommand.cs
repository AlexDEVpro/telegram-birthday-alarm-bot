using MediatR;

using Telegram.Bot.Types;

namespace TelegramBirthdayAlarmBot.Commands
{
    internal record CompleteSetCongratulateLangCommand(
        long ChatId,
        User From,
        string LanguageLabel) : IRequest;
}
