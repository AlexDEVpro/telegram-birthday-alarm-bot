using Telegram.Bot;
using Telegram.Bot.Types.Enums;

using TelegramBirthdayAlarmBot.Infrastructure.Authorization.Models;

namespace TelegramBirthdayAlarmBot.Infrastructure.Authorization.Services;

internal class TelegramGroupUserPermissionService
{
    private readonly ITelegramBotClient _bot;
    private readonly TelegramPermissionCache _cache;

    public TelegramGroupUserPermissionService(
        ITelegramBotClient bot,
        TelegramPermissionCache cache)
    {
        _bot = bot;
        _cache = cache;
    }

    public async Task<TelegramGroupUserPermissions> GetPermissionsAsync(
        long chatId,
        long userId)
    {
        if (_cache.TryGet(
            chatId,
            userId,
            out var cached))
        {
            return cached;
        }


        var member =
            await _bot.GetChatMember(
                chatId,
                userId);


        var result = new TelegramGroupUserPermissions(
            member.Status == ChatMemberStatus.Creator,
            member.Status == ChatMemberStatus.Administrator);


        _cache.Set(
            chatId,
            userId,
            result);

        return result;
    }
}