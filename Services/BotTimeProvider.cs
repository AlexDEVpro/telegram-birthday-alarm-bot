using Microsoft.Extensions.Options;
using TelegramBirthdayAlarmBot.Configuration;

namespace TelegramBirthdayAlarmBot.Services
{
    internal class BotTimeProvider
    {
        private readonly TimeZoneInfo _botTimeZone;

        public BotTimeProvider(IOptions<BirthdayOptions> options)
        {
            _botTimeZone = ResolveTimeZone(options.Value.TimeZone);
        }

        public DateTime Now => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _botTimeZone);

        private static TimeZoneInfo ResolveTimeZone(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return TimeZoneInfo.Utc;

            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(id);
            }
            catch
            {
                return TimeZoneInfo.Utc;
            }
        }
    }
}
