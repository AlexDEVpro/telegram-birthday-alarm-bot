namespace TelegramBirthdayAlarmBot.Infrastructure.Authorization.Models;

internal record TelegramGroupUserPermissions(
    bool IsOwner,
    bool IsAdmin);