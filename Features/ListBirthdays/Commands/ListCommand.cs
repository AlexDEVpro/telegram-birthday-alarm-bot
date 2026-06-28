using MediatR;

using Telegram.Bot.Types;

namespace TelegramBirthdayAlarmBot.Features.ListBirthdays.Commands;

internal record ListCommand(
    long ChatId,
    User From) : IRequest;
