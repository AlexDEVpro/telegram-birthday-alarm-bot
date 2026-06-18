using TelegramBirthdayAlarmBot.Infrastructure.Authorization.Models;

namespace TelegramBirthdayAlarmBot.Infrastructure.Authorization.Services;

internal class TelegramPermissionCache
{
    private readonly Dictionary<(long ChatId, long UserId), TelegramGroupUserPermissions>
        _cache = new();


    public bool TryGet(
        long chatId,
        long userId,
        out TelegramGroupUserPermissions permissions)
    {
        return _cache.TryGetValue(
            (chatId, userId),
            out permissions!);
    }


    public void Set(
        long chatId,
        long userId,
        TelegramGroupUserPermissions permissions)
    {
        _cache[(chatId, userId)] = permissions;
    }
}
