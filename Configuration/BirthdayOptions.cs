namespace TelegramBirthdayAlarmBot.Configuration
{
    internal class BirthdayOptions
    {
        public string DateCulture { get; set; } = "en-US";
        public int SendHour { get; set; } = 10;
        public string TimeZone { get; set; } = "UTC";
        public int LateWindowMonths { get; set; } = 2;
        public long CheckIntervalMinutes { get; set; } = 10;
    }
}