using MediatR;

using Microsoft.Extensions.DependencyInjection;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

using TelegramBirthdayAlarmBot.Infrastructure.Persistence.Services;
using TelegramBirthdayAlarmBot.Infrastructure.Telegram.Commands;

namespace TelegramBirthdayAlarmBot.Infrastructure.Telegram.Services;

internal class BotService
{
    private readonly ITelegramBotClient _bot;
    private readonly StorageService _storage;
    private readonly IServiceScopeFactory _scopeFactory;

    public BotService(
        IMediator mediator,
        ITelegramBotClient bot,
        StorageService storage,
        IServiceScopeFactory scopeFactory)
    {
        _bot = bot;
        _storage = storage;
        _scopeFactory = scopeFactory;
    }

    public void Start()
    {
        _bot.StartReceiving(HandleUpdate, HandleError);
    }

    private async Task HandleUpdate(ITelegramBotClient client, Update update, CancellationToken token)
    {
        if (update.Type != UpdateType.Message || update.Message?.Text == null)
            return;

        var chatId = update.Message.Chat.Id;
        var from = update.Message.From;
        if (from == null)
        {
            await _bot.SendMessage(chatId,
                Resources.BotMessages.ErrorMessageFromIsNull,
                disableNotification: true);

            return;
        }

        var text = update.Message.Text.Trim();

        // Add the user ID if missing and update the username if it has changed.
        if (!string.IsNullOrEmpty(from.Username))
        {
            _storage.UpdateUserIdentity(from.Id, "@" + from.Username);
        }

        // Pass message to MediatR.
        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        await mediator.Send(new IncomingUpdateCommand(chatId, from, text), token);
    }

    private Task HandleError(ITelegramBotClient client, Exception ex, CancellationToken token)
    {
        Console.WriteLine(ex);

        return Task.CompletedTask;
    }
}