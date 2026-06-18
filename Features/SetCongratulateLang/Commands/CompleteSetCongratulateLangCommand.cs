using MediatR;

using Telegram.Bot.Types;

namespace TelegramBirthdayAlarmBot.Features.SetCongratulateLang.Commands
{
    internal record CompleteSetCongratulateLangCommand(
        long ChatId,
        User From,
        string LanguageLabel) : IRequest;
}
