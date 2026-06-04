using MediatR;

using TelegramBirthdayAlarmBot.Commands;
using TelegramBirthdayAlarmBot.Constants;
using TelegramBirthdayAlarmBot.Services;
using TelegramBirthdayAlarmBot.Services.Localization;

namespace TelegramBirthdayAlarmBot.Handlers
{
    internal class IncomingUpdateHandler : IRequestHandler<IncomingUpdateCommand>
    {
        private readonly IMediator _mediator;
        private readonly UserCultureResolver _userCultureResolver;
        private readonly CultureContextManager _cultureContextManager;
        private readonly PendingAddBirthdayStateService _pendingAddBirthdayStateService;
        private readonly PendingSetCongratulateLangStateService _pendingSetCongratulateLangStateService;

        public IncomingUpdateHandler(
            IMediator mediator,
            UserCultureResolver userCultureResolver,
            CultureContextManager cultureContextManager,
            PendingAddBirthdayStateService pendingAddBirthdayStateService,
            PendingSetCongratulateLangStateService pendingSetCongratulateLangStateService)
        {
            _mediator = mediator;
            _userCultureResolver = userCultureResolver;
            _cultureContextManager = cultureContextManager;
            _pendingAddBirthdayStateService = pendingAddBirthdayStateService;
            _pendingSetCongratulateLangStateService = pendingSetCongratulateLangStateService;
        }

        public async Task Handle(IncomingUpdateCommand request, CancellationToken cancellationToken)
        {
            var chatId = request.ChatId;
            var from = request.From;
            var text = request.Text;

            using (_cultureContextManager.Use(_userCultureResolver.Resolve(from.LanguageCode)))
            {
                // Cancel of add birthday interactive mode.
                if (text.StartsWith($"/{BotCommands.Cancel}"))
                {
                    await _mediator.Send(new CancelCommand(chatId, from), cancellationToken);

                    return;
                }

                // Add birthday interactive mode. Step 2.
                if (_pendingAddBirthdayStateService.IsPending(chatId, from.Id))
                {
                    await _mediator.Send(new CompleteAddBirthdayCommand(chatId, from, text), cancellationToken);

                    return;
                }
                // Set congratulate lang interactive mode. Step 2.
                if (_pendingSetCongratulateLangStateService.IsPending(chatId, from.Id))
                {
                    await _mediator.Send(
                        new CompleteSetCongratulateLangCommand(
                            chatId,
                            from,
                            text));

                    return;
                }

                if (text.StartsWith($"/{BotCommands.AddBirthday}"))
                {
                    await _mediator.Send(new AddBirthdayCommand(chatId, from, text), cancellationToken);
                }
                else if (text.StartsWith($"/{BotCommands.RemoveBirthday}"))
                {
                    await _mediator.Send(new RemoveBirthdayCommand(chatId, from, text), cancellationToken);
                }
                else if (text.StartsWith($"/{BotCommands.List}"))
                {
                    await _mediator.Send(new ListCommand(chatId, from), cancellationToken);
                }
                else if (text.StartsWith($"/{BotCommands.SetCongratulateLang}"))
                {
                    await _mediator.Send(new SetCongratulateLangCommand(chatId, from), cancellationToken);
                }
                else if (text.StartsWith($"/{BotCommands.Help}"))
                {
                    await _mediator.Send(new HelpCommand(chatId, from), cancellationToken);
                }
            }
        }
    }
}
