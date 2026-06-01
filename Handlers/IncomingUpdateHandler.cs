using MediatR;

using TelegramBirthdayAlarmBot.Commands;
using TelegramBirthdayAlarmBot.Constants;
using TelegramBirthdayAlarmBot.Services;

namespace TelegramBirthdayAlarmBot.Handlers
{
    internal class IncomingUpdateHandler : IRequestHandler<IncomingUpdateCommand>
    {
        private readonly IMediator _mediator;
        private readonly PendingAddStateService _stateService;

        public IncomingUpdateHandler(IMediator mediator, PendingAddStateService stateService)
        {
            _mediator = mediator;
            _stateService = stateService;
        }

        public async Task Handle(IncomingUpdateCommand request, CancellationToken cancellationToken)
        {
            var chatId = request.ChatId;
            var from = request.From;
            var text = request.Text;

            // Cancel of add birthday interactive mode.
            if (text.StartsWith($"/{BotCommands.Cancel}"))
            {
                await _mediator.Send(new CancelCommand(chatId, from), cancellationToken);

                return;
            }

            // Add birthday interactive mode. Step 2.
            if (_stateService.IsPending(chatId, from.Id))
            {
                await _mediator.Send(new CompleteAddBirthdayCommand(chatId, from, text), cancellationToken);

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
            else if (text.StartsWith($"/{BotCommands.Help}"))
            {
                await _mediator.Send(new HelpCommand(chatId, from), cancellationToken);
            }
        }
    }
}
