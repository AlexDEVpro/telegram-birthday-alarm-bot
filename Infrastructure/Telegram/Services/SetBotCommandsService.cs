using System.Globalization;
using System.Resources;

using Microsoft.Extensions.Hosting;

using Telegram.Bot;
using Telegram.Bot.Types;

using TelegramBirthdayAlarmBot.Constants;

namespace TelegramBirthdayAlarmBot.Infrastructure.Telegram.Services;

internal class SetBotCommandsService : IHostedService
{
    private readonly ITelegramBotClient _bot;

    private readonly ResourceManager _resources =
        new(typeof(Resources.BotMessages));

    public SetBotCommandsService(
        ITelegramBotClient bot)
    {
        _bot = bot;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (var lang in SupportedLanguages.All)
        {
            await RegisterCommands(
                lang.Code,
                new CultureInfo(lang.Culture),
                cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;

    private async Task RegisterCommands(
        string languageCode,
        CultureInfo culture,
        CancellationToken cancellationToken)
    {
        await _bot.SetMyCommands(
            CreateCommands(culture, true),
            scope: new BotCommandScopeAllChatAdministrators(),
            languageCode: languageCode,
            cancellationToken: cancellationToken);

        await _bot.SetMyCommands(
            CreateCommands(culture, false),
            scope: new BotCommandScopeDefault(),
            languageCode: languageCode,
            cancellationToken: cancellationToken);
    }

    private BotCommand[] CreateCommands(
        CultureInfo culture,
        bool isAdmin)
    {
        return
        [
            new()
            {
                Command = BotCommands.AddBirthday,
                Description = GetString(
                    isAdmin
                        ? nameof(Resources.BotMessages.BotCommandAddBirthdayAdmin)
                        : nameof(Resources.BotMessages.BotCommandAddBirthday),
                    culture)
            },

            new()
            {
                Command = BotCommands.RemoveBirthday,
                Description = GetString(
                    isAdmin
                        ? nameof(Resources.BotMessages.BotCommandRemoveBirthdayAdmin)
                        : nameof(Resources.BotMessages.BotCommandRemoveBirthday),
                    culture)
            },

            new()
            {
                Command = BotCommands.List,
                Description = GetString(
                    nameof(Resources.BotMessages.BotCommandList),
                    culture)
            },

            new()
            {
                Command = BotCommands.Cancel,
                Description = GetString(
                    nameof(Resources.BotMessages.BotCommandCancel),
                    culture)
            },

            new()
            {
                Command = BotCommands.SetCongratulateLang,
                Description = GetString(
                    nameof(Resources.BotMessages.BotCommandSetCongratulateLangOwner),
                    culture)
            },

            new()
            {
                Command = BotCommands.Help,
                Description = GetString(
                    isAdmin
                        ? nameof(Resources.BotMessages.BotCommandHelpAdmin)
                        : nameof(Resources.BotMessages.BotCommandHelp),
                    culture)
            }
        ];
    }

    private string GetString(string resourceName,
        CultureInfo culture)
    {
        return _resources.GetString(resourceName, culture)!;
    }
}