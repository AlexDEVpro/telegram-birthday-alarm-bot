using MediatR;

using Telegram.Bot.Types;

namespace TelegramBirthdayAlarmBot.Features.AddBirthday.Commands;

internal record AddBirthdayCommand(
    long ChatId,
    User From,
    string Text) : IRequest;
