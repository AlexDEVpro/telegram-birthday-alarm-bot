using Microsoft.Extensions.Options;

using TelegramBirthdayAlarmBot.Configuration;
using TelegramBirthdayAlarmBot.Infrastructure.Authorization.Models;

namespace TelegramBirthdayAlarmBot.Infrastructure.Authorization.Services;

internal class BotPermissionService
{
    private readonly HashSet<long> _botAdminIDs;
    private readonly bool _allowTelegramGroupAdmins;

    private readonly TelegramGroupUserPermissionService
        _telegramPermissions;

    public BotPermissionService(
        IOptions<TelegramOptions> telegramOptions,
        IOptions<PermissionOptions> permissionOptions,
        TelegramGroupUserPermissionService telegramGroupUserPermissions)
    {
        _telegramPermissions = telegramGroupUserPermissions;

        _botAdminIDs =
            telegramOptions.Value.AdminIDs.ToHashSet();

        _allowTelegramGroupAdmins =
            permissionOptions.Value.AllowTelegramGroupAdmins;
    }

    public bool AllowTelegramGroupAdmins => _allowTelegramGroupAdmins;

    public async Task<bool> HasPermissionAsync(
        long chatId,
        long userId,
        BotPermission permission)
    {
        if (_botAdminIDs.Contains(userId))
            return true;

        var telegramPermissions =
            await _telegramPermissions
                .GetPermissionsAsync(chatId, userId);

        return permission switch
        {
            BotPermission.ManageOtherBirthdays =>
                telegramPermissions.IsOwner
                || (_allowTelegramGroupAdmins && telegramPermissions.IsAdmin),

            BotPermission.SetCongratulateLang =>
                telegramPermissions.IsOwner,

            _ => false
        };
    }
}