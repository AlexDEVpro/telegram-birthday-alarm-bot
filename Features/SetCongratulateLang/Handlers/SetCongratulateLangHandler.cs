using MediatR;

using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

using TelegramBirthdayAlarmBot.Constants;
using TelegramBirthdayAlarmBot.Features.SetCongratulateLang.Commands;
using TelegramBirthdayAlarmBot.Features.SetCongratulateLang.Services;
using TelegramBirthdayAlarmBot.Infrastructure.Authorization.Models;
using TelegramBirthdayAlarmBot.Infrastructure.Authorization.Services;

namespace TelegramBirthdayAlarmBot.Features.SetCongratulateLang.Handlers;

internal class SetCongratulateLangHandler
: IRequestHandler<SetCongratulateLangCommand>
{
    private readonly ITelegramBotClient _bot;
    private readonly PendingSetCongratulateLangStateService _pendingSetCongratulateLangStateService;
    private readonly BotPermissionService _permissionService;

    public SetCongratulateLangHandler(
        ITelegramBotClient bot,
        PendingSetCongratulateLangStateService pendingSetCongratulateLangStateService,
        BotPermissionService permissionService)
    {
        _bot = bot;
        _pendingSetCongratulateLangStateService = pendingSetCongratulateLangStateService;
        _permissionService = permissionService;
    }

    public async Task Handle(
        SetCongratulateLangCommand request,
        CancellationToken cancellationToken)
    {
        var chatId = request.ChatId;
        var from = request.From;

        if (!await _permissionService.HasPermissionAsync(
                chatId,
                from.Id,
                BotPermission.SetCongratulateLang))
        {
            await _bot.SendMessage(
                chatId,
                Resources.BotMessages.SetCongratulateLangOwnerOnly,
                disableNotification: true);

            return;
        }

        _pendingSetCongratulateLangStateService.Begin(chatId, from.Id);

        await _bot.SendMessage(
            chatId,
            Resources.BotMessages.ChooseCongratulateLang,
            replyMarkup: new ReplyKeyboardMarkup([
                SupportedLanguages.All
                    .Select(x => new KeyboardButton(x.Label))
                    .ToArray(),
                [ new($"/{BotCommands.Cancel}") ]
            ])
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = true
            },
            disableNotification: true);
    }
}
