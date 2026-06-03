using System.Globalization;
using System.Resources;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using Telegram.Bot;
using Telegram.Bot.Types;

using TelegramBirthdayAlarmBot.Configuration;
using TelegramBirthdayAlarmBot.Constants;

namespace TelegramBirthdayAlarmBot.Services
{
    internal class SetBotCommandsService : IHostedService
    {
        private readonly ITelegramBotClient _bot;
        private readonly PermissionOptions _permissionOptions;

        private readonly ResourceManager _resources =
            new(typeof(Resources.BotMessages));

        public SetBotCommandsService(
            ITelegramBotClient bot,
            IOptions<PermissionOptions> permissionOptions)
        {
            _bot = bot;
            _permissionOptions = permissionOptions.Value;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var lang in SupportedLanguages.All)
            {
                await RegisterCommands(
                    languageCode: lang.Code,
                    culture: new CultureInfo(lang.Culture),
                    isAdmin: _permissionOptions.AllowTelegramGroupAdmins,
                    cancellationToken);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;

        private async Task RegisterCommands(
            string languageCode,
            CultureInfo culture,
            bool isAdmin,
            CancellationToken cancellationToken)
        {
            await _bot.SetMyCommands(
                CreateCommands(culture, isAdmin),
                scope: isAdmin
                    ? new BotCommandScopeAllChatAdministrators()
                    : new BotCommandScopeDefault(),
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

                ..(isAdmin
                    ? new[]
                    {
                        new BotCommand
                        {
                            Command = BotCommands.SetCongratulateLang,
                            Description = GetString(
                                nameof(Resources.BotMessages.BotCommandSetCongratulateLangOwner),
                                culture)
                        }
                    }
                    : []),

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
}