using Microsoft.Extensions.Options;
using System.Globalization;
using TelegramBirthdayAlarmBot.Configuration;

namespace TelegramBirthdayAlarmBot.Services.Localization;

internal class UserCultureResolver
{
    private readonly string _defaultCulture;

    public UserCultureResolver(IOptions<BirthdayOptions> options)
    {
        _defaultCulture = options.Value.DateCulture;
    }

    public CultureInfo Resolve(string? telegramLangCode)
    {
        var fallback = _defaultCulture ?? "en-US";

        if (string.IsNullOrWhiteSpace(telegramLangCode))
            return new CultureInfo(fallback);

        try
        {
            return telegramLangCode switch
            {
                "en" => new CultureInfo("en-US"),
                "ru" => new CultureInfo("ru-RU"),
                _ => new CultureInfo(fallback)
            };
        }
        catch
        {
            return new CultureInfo(fallback);
        }
    }
}