using System.Globalization;
using System.Resources;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using Telegram.Bot;
using Telegram.Bot.Types;

using TelegramBirthdayAlarmBot.Configuration;
using TelegramBirthdayAlarmBot.Constants;

namespace TelegramBirthdayAlarmBot.Services;

internal class SetBotCommandsService : IHostedService
{
    private readonly ITelegramBotClient _bot;
    private readonly PermissionOptions _permissionOptions;

    public SetBotCommandsService(
        ITelegramBotClient bot,
        IOptions<PermissionOptions> permissionOptions)
    {
        _bot = bot;
        _permissionOptions = permissionOptions.Value;
    }

    public async Task StartAsync(
        CancellationToken cancellationToken)
    {
        await RegisterDefaultCommands(cancellationToken);

        if (_permissionOptions.AllowTelegramGroupAdmins)
        {
            await RegisterTelegramAdminCommands(
                cancellationToken);
        }
    }

    public Task StopAsync(
        CancellationToken cancellationToken)
        => Task.CompletedTask;

    private async Task RegisterDefaultCommands(
        CancellationToken cancellationToken)
    {
        var r = new ResourceManager(typeof(Resources.BotMessages));
        var cultureEn = new CultureInfo("en-US");
        var cultureRu = new CultureInfo("ru-RU");

        await _bot.SetMyCommands(
        [
            new() { Command = BotCommands.AddBirthday, Description = r.GetString(nameof(Resources.BotMessages.BotCommandAddBirthday), cultureEn)! },
            new() { Command = BotCommands.RemoveBirthday, Description = r.GetString(nameof(Resources.BotMessages.BotCommandRemoveBirthday), cultureEn)! },
            new() { Command = BotCommands.List, Description = r.GetString(nameof(Resources.BotMessages.BotCommandList), cultureEn)! },
            new() { Command = BotCommands.Cancel, Description = r.GetString(nameof(Resources.BotMessages.BotCommandCancel), cultureEn)! },
            new() { Command = BotCommands.Help, Description = r.GetString(nameof(Resources.BotMessages.BotCommandHelp), cultureEn)! }
        ],
        scope: new BotCommandScopeDefault(),
        languageCode: "en",
        cancellationToken: cancellationToken);

        await _bot.SetMyCommands(
        [
            new() { Command = BotCommands.AddBirthday, Description = r.GetString(nameof(Resources.BotMessages.BotCommandAddBirthday), cultureRu)! },
            new() { Command = BotCommands.RemoveBirthday, Description = r.GetString(nameof(Resources.BotMessages.BotCommandRemoveBirthday), cultureRu)! },
            new() { Command = BotCommands.List, Description = r.GetString(nameof(Resources.BotMessages.BotCommandList), cultureRu)! },
            new() { Command = BotCommands.Cancel, Description = r.GetString(nameof(Resources.BotMessages.BotCommandCancel), cultureRu)! },
            new() { Command = BotCommands.Help, Description = r.GetString(nameof(Resources.BotMessages.BotCommandHelp), cultureRu)! }
        ],
        scope: new BotCommandScopeDefault(),
        languageCode: "ru",
        cancellationToken: cancellationToken);
    }

    private async Task RegisterTelegramAdminCommands(
        CancellationToken cancellationToken)
    {
        var r = new ResourceManager(typeof(Resources.BotMessages));
        var cultureEn = new CultureInfo("en-US");
        var cultureRu = new CultureInfo("ru-RU");

        await _bot.SetMyCommands(
        [
            new() { Command = BotCommands.AddBirthday, Description = r.GetString(nameof(Resources.BotMessages.BotCommandAddBirthdayAdmin), cultureEn)! },
            new() { Command = BotCommands.RemoveBirthday, Description = r.GetString(nameof(Resources.BotMessages.BotCommandRemoveBirthdayAdmin), cultureEn)! },
            new() { Command = BotCommands.List, Description = r.GetString(nameof(Resources.BotMessages.BotCommandList), cultureEn)! },
            new() { Command = BotCommands.Cancel, Description = r.GetString(nameof(Resources.BotMessages.BotCommandCancel), cultureEn)! },
            new() { Command = BotCommands.Help, Description = r.GetString(nameof(Resources.BotMessages.BotCommandHelpAdmin), cultureEn)! }
        ],
        scope: new BotCommandScopeAllChatAdministrators(),
        languageCode: "en",
        cancellationToken: cancellationToken);

        await _bot.SetMyCommands(
        [
            new() { Command = BotCommands.AddBirthday, Description = r.GetString(nameof(Resources.BotMessages.BotCommandAddBirthdayAdmin), cultureRu)! },
            new() { Command = BotCommands.RemoveBirthday, Description = r.GetString(nameof(Resources.BotMessages.BotCommandRemoveBirthdayAdmin), cultureRu)! },
            new() { Command = BotCommands.List, Description = r.GetString(nameof(Resources.BotMessages.BotCommandList), cultureRu)! },
            new() { Command = BotCommands.Cancel, Description = r.GetString(nameof(Resources.BotMessages.BotCommandCancel), cultureRu)! },
            new() { Command = BotCommands.Help, Description = r.GetString(nameof(Resources.BotMessages.BotCommandHelpAdmin), cultureRu)! }
        ],
        scope: new BotCommandScopeAllChatAdministrators(),
        languageCode: "ru",
        cancellationToken: cancellationToken);
    }
}