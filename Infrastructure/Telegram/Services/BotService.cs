using MediatR;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

using TelegramBirthdayAlarmBot.Infrastructure.Persistence.Services;
using TelegramBirthdayAlarmBot.Infrastructure.Telegram.Commands;

namespace TelegramBirthdayAlarmBot.Infrastructure.Telegram.Services;

internal class BotService : BackgroundService
{
    private readonly ITelegramBotClient _bot;
    private readonly StorageService _storage;
    private readonly IServiceScopeFactory _scopeFactory;

    public BotService(
        ITelegramBotClient bot,
        StorageService storage,
        IServiceScopeFactory scopeFactory)
    {
        _bot = bot;
        _storage = storage;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken hostCt)
    {
        while (!hostCt.IsCancellationRequested) // While host isn't stopping.
        {
            using var receiverCts =
                CancellationTokenSource.CreateLinkedTokenSource(hostCt);

            _bot.StartReceiving(
                HandleUpdate,
                (client, exception, ct) =>
                    HandleError(client, exception, ct, receiverCts),
                cancellationToken: receiverCts.Token);

            Console.WriteLine("Connected to Telegram.");

            try
            {
                await Task.Delay(
                    Timeout.InfiniteTimeSpan,
                    receiverCts.Token);
            }
            catch (OperationCanceledException)
            {
                if (hostCt.IsCancellationRequested)
                    break; // Exit if the cancellation was requested by the host.
            }

            if (!hostCt.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(
                        TimeSpan.FromSeconds(5),
                        hostCt);
                }
                catch (OperationCanceledException)
                {
                    break; // Exit if the cancellation was requested by the host.
                }
            }
        }
    }

    private async Task HandleUpdate(
        ITelegramBotClient client,
        Update update,
        CancellationToken ct)
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
        await mediator.Send(new IncomingUpdateCommand(chatId, from, text), ct);
    }

    private async Task HandleError(
        ITelegramBotClient client,
        Exception ex,
        CancellationToken ct,
        CancellationTokenSource receiverCts)
    {
        if (ct.IsCancellationRequested)
            return;

        if (ShouldReconnect(ex))
        {
            Console.WriteLine("HTTPS error. Reconnecting to Telegram...");

            await receiverCts.CancelAsync();

            return;
        }

        Console.WriteLine(ex);
    }

    private static bool ShouldReconnect(Exception exception)
    {
        for (var current = exception;
             current != null;
             current = current.InnerException)
        {
            if (current is HttpRequestException or IOException or TimeoutException)
                return true;

            if (current is TaskCanceledException)
                return true;

            if (current is RequestException requestException &&
                IsTransientRequestException(requestException))
                return true;
        }

        return false;
    }

    private static bool IsTransientRequestException(
        RequestException exception)
    {
        return exception.Message.Contains(
            "timed out",
            StringComparison.OrdinalIgnoreCase);
    }
}
