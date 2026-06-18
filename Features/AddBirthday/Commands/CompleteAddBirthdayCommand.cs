using MediatR;

using Telegram.Bot.Types;

namespace TelegramBirthdayAlarmBot.Features.AddBirthday.Commands;

internal record CompleteAddBirthdayCommand(
    long ChatId,
    User From,
    string Text) : IRequest;
