using System.Globalization;

using Microsoft.Extensions.Options;

using TelegramBirthdayAlarmBot.Configuration;
using TelegramBirthdayAlarmBot.Constants;

namespace TelegramBirthdayAlarmBot.Infrastructure.Localization.Services;

internal class UserCultureResolver
{
    private readonly string _defaultLanguageCode;

    public UserCultureResolver(IOptions<BirthdayOptions> birthdayOptions)
    {
        _defaultLanguageCode = birthdayOptions.Value.DefaultLanguageCode;
    }

    public CultureInfo Resolve(string? telegramLanguageCode)
    {
        var language =
            string.IsNullOrWhiteSpace(telegramLanguageCode)
                ? SupportedLanguages.Get(_defaultLanguageCode)
                : SupportedLanguages.Get(telegramLanguageCode);

        return new CultureInfo(language.Culture);
    }
}