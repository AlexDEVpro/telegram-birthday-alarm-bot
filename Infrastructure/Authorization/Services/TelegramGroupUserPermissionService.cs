using Telegram.Bot;
using Telegram.Bot.Types.Enums;

using TelegramBirthdayAlarmBot.Infrastructure.Authorization.Models;

namespace TelegramBirthdayAlarmBot.Infrastructure.Authorization.Services;

internal class TelegramGroupUserPermissionService
{
    private readonly ITelegramBotClient _bot;

    public TelegramGroupUserPermissionService(
        ITelegramBotClient bot)
    {
        _bot = bot;
    }

    public async Task<TelegramGroupUserPermissions> GetPermissionsAsync(
        long chatId,
        long userId)
    {
        var member = await _bot.GetChatMember(
            chatId,
            userId);

        return member.Status switch
        {
            ChatMemberStatus.Creator => new TelegramGroupUserPermissions(
                IsOwner: true,
                IsAdmin: true),

            ChatMemberStatus.Administrator => new TelegramGroupUserPermissions(
                IsOwner: false,
                IsAdmin: true),

            _ => new TelegramGroupUserPermissions(
                IsOwner: false,
                IsAdmin: false)
        };
    }
}