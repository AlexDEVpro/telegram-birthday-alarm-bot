namespace TelegramBirthdayAlarmBot.Configuration;

internal class TelegramOptions
{
    public string BotToken { get; set; } = "";
    public long[] AdminIDs { get; set; } = [];
}