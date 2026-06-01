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
        await _bot.SetMyCommands(
        [
            new() { Command = BotCommands.AddBirthday, Description = "Add your birthday" },
            new() { Command = BotCommands.RemoveBirthday, Description = "Remove your birthday" },
            new() { Command = BotCommands.List, Description = "List of all birthdays" },
            new() { Command = BotCommands.Cancel, Description = "Cancel birthday adding operation" },
            new() { Command = BotCommands.Help, Description = "Command format and tips" }
        ],
        scope: new BotCommandScopeDefault(),
        languageCode: "en",
        cancellationToken: cancellationToken);

        await _bot.SetMyCommands(
        [
            new() { Command = BotCommands.AddBirthday, Description = "Добавить свой ДР" },
            new() { Command = BotCommands.RemoveBirthday, Description = "Удалить свой ДР" },
            new() { Command = BotCommands.List, Description = "Список всех ДР" },
            new() { Command = BotCommands.Cancel, Description = "Отмена операции добавления ДР" },
            new() { Command = BotCommands.Help, Description = "Формат команд и подсказки" }
        ],
        scope: new BotCommandScopeDefault(),
        languageCode: "ru",
        cancellationToken: cancellationToken);
    }

    private async Task RegisterTelegramAdminCommands(
        CancellationToken cancellationToken)
    {
        await _bot.SetMyCommands(
        [
            new() { Command = BotCommands.AddBirthday, Description = "Add your birthday or another user's birthday" },
            new() { Command = BotCommands.RemoveBirthday, Description = "Remove your birthday or another user's birthday" },
            new() { Command = BotCommands.List, Description = "List of all birthdays" },
            new() { Command = BotCommands.Cancel, Description = "Cancel birthday adding operation" },
            new() { Command = BotCommands.Help, Description = "Command format and tips" }
        ],
        scope: new BotCommandScopeAllChatAdministrators(),
        languageCode: "en",
        cancellationToken: cancellationToken);

        await _bot.SetMyCommands(
        [
            new() { Command = BotCommands.AddBirthday, Description = "Добавить свой ДР или ДР другого пользователя" },
            new() { Command = BotCommands.RemoveBirthday, Description = "Удалить свой ДР или ДР другого пользователя" },
            new() { Command = BotCommands.List, Description = "Список всех ДР" },
            new() { Command = BotCommands.Cancel, Description = "Отмена операции добавления ДР" },
            new() { Command = BotCommands.Help, Description = "Формат команд и подсказки" }
        ],
        scope: new BotCommandScopeAllChatAdministrators(),
        languageCode: "ru",
        cancellationToken: cancellationToken);
    }
}