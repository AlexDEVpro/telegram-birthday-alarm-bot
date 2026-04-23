using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBirthdayAlarmBot.Constants;

namespace TelegramBirthdayAlarmBot.Services
{
    internal class SetBotCommandsService : IHostedService
    {
        private readonly ITelegramBotClient _bot;

        public SetBotCommandsService(ITelegramBotClient bot)
        {
            _bot = bot;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _bot.SetMyCommands(new[]
            {
                new BotCommand { Command = BotCommands.AddBirthday, Description = "Add your birthday" },
                new BotCommand { Command = BotCommands.RemoveBirthday, Description = "Remove your birthday" },
                new BotCommand { Command = BotCommands.List, Description = "List of all birthdays" },
                new BotCommand { Command = BotCommands.Cancel, Description = "Cancel birthday adding operation" },
                new BotCommand { Command = BotCommands.Help, Description = "Command format and tips" }
            }, languageCode: "en");

            await _bot.SetMyCommands(new[]
            {
                new BotCommand { Command = BotCommands.AddBirthday, Description = "Добавить свой ДР" },
                new BotCommand { Command = BotCommands.RemoveBirthday, Description = "Удалить свой ДР" },
                new BotCommand { Command = BotCommands.List, Description = "Список всех ДР" },
                new BotCommand { Command = BotCommands.Cancel, Description = "Отмена операции добавления ДР" },
                new BotCommand { Command = BotCommands.Help, Description = "Формат команд и подсказки" }
            }, languageCode: "ru");
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
