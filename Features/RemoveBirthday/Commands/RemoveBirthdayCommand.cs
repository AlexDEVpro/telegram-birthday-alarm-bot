using MediatR;

using Telegram.Bot.Types;

namespace TelegramBirthdayAlarmBot.Features.RemoveBirthday.Commands;

internal record RemoveBirthdayCommand(
    long ChatId,
    User From,
    string Text) : IRequest;
