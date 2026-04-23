using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using TelegramBirthdayAlarmBot.Configuration;
using TelegramBirthdayAlarmBot.Models;

namespace TelegramBirthdayAlarmBot.Services
{
    internal class BirthdayService : BackgroundService
    {
        private readonly ITelegramBotClient _bot;
        private readonly BotTimeProvider _botTimeProvider;
        private readonly StorageService _storage;
        private readonly BirthdayOptions _options;

        public BirthdayService(ITelegramBotClient bot, BotTimeProvider botTimeProvider, StorageService storage, IOptions<BirthdayOptions> options)
        {
            _bot = bot;
            _botTimeProvider = botTimeProvider;
            _storage = storage;
            _options = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    await Check();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }

                await Task.Delay(TimeSpan.FromMinutes(_options.CheckIntervalMinutes), ct);
            }
        }

        private async Task Check()
        {
            var now = _botTimeProvider.Now;

            var birthdays = _storage.GetAllBirthdaysSnapshot();

            foreach (var (chatId, userKey, bs) in birthdays)
            {
                var decision = ShouldSendMessage(bs, now);

                switch (decision)
                {
                    case SendDecision.SendNormal:
                    case SendDecision.SendLate:
                        string mention;
                        if (bs.UserId != null)
                        {
                            mention = $"<a href=\"tg://user?id={bs.UserId}\">{bs.DisplayName}</a>";
                        }
                        else
                        {
                            mention = bs.DisplayName;
                        }

                        var sb = new StringBuilder();
                        sb.AppendFormat(Resources.BotMessages.BirthdayCongrats, mention);

                        if (decision == SendDecision.SendLate)
                        {
                            sb.AppendLine();
                            sb.Append(Resources.BotMessages.LateBirthdayCongratsAppendix);
                        }

                        try
                        {
                            await _bot.SendMessage(chatId, sb.ToString(), parseMode: ParseMode.Html);

                            if(!_storage.MarkCelebrated(chatId, userKey, now.Year))
                            {
                                Console.Error.WriteLine($"Check - unable to mark celebrated chatID: {chatId}, user key: {userKey}, new celebrated year: {now.Year}.");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.Error.WriteLine(ex);
                        }

                        break;

                    case SendDecision.SkipAlreadySent:
                    case SendDecision.SkipTooOld:
                    case SendDecision.SkipLateTooEarly:
                    case SendDecision.SkipTodayTooEarly:
                    case SendDecision.SkipTooEarly:
                    default:
                        //Console.WriteLine($"Check - chat ID: {chatId}, user key: {userKey}, skip because of decision: {decision}.");

                        continue;
                }
            }
        }

        private SendDecision ShouldSendMessage(BirthdayState bs, DateTime now)
        {
            // Already processed this year.
            if (bs.LastCelebratedYear == now.Year)
                return SendDecision.SkipAlreadySent;

            var birthdayThisYear = new DateTime(now.Year, bs.Date.Month, bs.Date.Day);

            // Too late cutoff.
            if (birthdayThisYear < now.AddMonths(-_options.LateWindowMonths).Date)
                return SendDecision.SkipTooOld;

            var sendTime = birthdayThisYear.AddHours(_options.SendHour);

            // Today case.
            if (now.Date == birthdayThisYear.Date)
            {
                if (now < sendTime)
                    return SendDecision.SkipTodayTooEarly;

                return SendDecision.SendNormal;
            }

            // Late case (missed birthday, but still within allowed window).
            if (now.Date > birthdayThisYear.Date)
            {
                if (now < sendTime)
                    return SendDecision.SkipLateTooEarly;

                return SendDecision.SendLate;
            }

            // Future birthdays - do nothing.
            return SendDecision.SkipTooEarly;
        }
    }
}
