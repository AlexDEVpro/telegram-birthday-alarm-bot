using System.Text.Json;

using TelegramBirthdayAlarmBot.Constants;
using TelegramBirthdayAlarmBot.Infrastructure.Persistence.Models;

namespace TelegramBirthdayAlarmBot.Infrastructure.Persistence.Services;

internal class StorageService
{
    private readonly string _file = Path.Combine(AppContext.BaseDirectory, "Data", "birthdays.json");
    private readonly object _lock = new();

    private BotData _data;

    public StorageService()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_file)!);
        _data = Load();
    }

    public List<(long ChatId, string UserKey, BirthdayState State)> GetAllBirthdaysSnapshot()
    {
        lock (_lock)
        {
            var result = new List<(long, string, BirthdayState)>();

            foreach (var (chatId, chat) in _data.Chats)
            {
                foreach (var (userKey, bs) in chat.Users)
                {
                    result.Add((chatId, userKey, bs));
                }
            }

            return result;
        }
    }

    public bool AddBirthday(
        long chatId,
        long? userId,
        string usernameOrDisplayName,
        DateTime date)
    {
        lock (_lock)
        {
            if (!_data.Chats.ContainsKey(chatId))
                _data.Chats[chatId] = new ChatData();

            var users = _data.Chats[chatId].Users;
            var (username, displayName) = ParseIdentity(usernameOrDisplayName);

            // Dictionary key.
            var userKey = BuildKey(username, displayName);

            // Duplicate check.
            if (users.ContainsKey(userKey))
                return false;
            if (userId != null && users.Values.Any(x => x.UserId == userId))
                return false;

            // If birthday already passed this year then mark as already celebrated.
            var lastCelebratedYear = 0;
            var now = DateTime.UtcNow;
            var currentYear = now.Year;
            var birthdayThisYear = new DateTime(currentYear, date.Month, date.Day);
            if (birthdayThisYear < now.Date)
            {
                lastCelebratedYear = currentYear;
            }

            users[userKey] = new BirthdayState
            {
                UserId = userId,
                Username = username,
                DisplayName = displayName,
                Date = date,
                LastCelebratedYear = lastCelebratedYear
            };

            Save();

            return true;
        }
    }

    public List<BirthdayState> GetSortedBirthdays(long chatId)
    {
        lock (_lock)
        {
            if (!_data.Chats.ContainsKey(chatId))
                return new();

            return _data.Chats[chatId].Users.Values
                .OrderBy(x => x.Date.Month)
                .ThenBy(x => x.Date.Day)
                .ToList();
        }
    }

    public bool RemoveBirthday(long chatId, long userId)
    {
        lock (_lock)
        {
            if (!_data.Chats.ContainsKey(chatId))
                return false;

            var users = _data.Chats[chatId].Users;

            // Find user by user ID.
            var user = users.FirstOrDefault(x => x.Value.UserId == userId);
            if (user.Key == null)
                return false;

            var removed = users.Remove(user.Key);
            if (removed)
                Save();

            return removed;
        }
    }
    public bool RemoveBirthday(long chatId, string usernameOrDisplayName)
    {
        lock (_lock)
        {
            if (!_data.Chats.ContainsKey(chatId))
                return false;

            var (username, displayName) = ParseIdentity(usernameOrDisplayName);
            var userKey = BuildKey(username, displayName);

            var removed = _data.Chats[chatId].Users.Remove(userKey);

            if (removed)
                Save();

            return removed;
        }
    }

    public void UpdateUserIdentity(long userId, string username)
    {
        lock (_lock)
        {
            username = username.ToLowerInvariant();

            foreach (var chat in _data.Chats.Values)
            {
                foreach (var user in chat.Users.Values)
                {
                    if (user.UserId == userId)
                    {
                        user.Username = username;

                        break;
                    }
                    else if (user.Username?.ToLowerInvariant() == username)
                    {
                        user.UserId = userId;

                        break;
                    }
                }
            }

            Save();
        }
    }

    public bool MarkCelebrated(long chatId, string userKey, int newLastCelebratedYear)
    {
        lock (_lock)
        {
            if (_data.Chats.ContainsKey(chatId) &&
                _data.Chats[chatId].Users.ContainsKey(userKey))
            {
                _data.Chats[chatId].Users[userKey].LastCelebratedYear = newLastCelebratedYear;

                Save();

                return true;
            }

            return false;
        }
    }

    public string GetCongratulateCulture(long chatId)
    {
        lock (_lock)
        {
            if (_data.Chats.ContainsKey(chatId))
                return _data.Chats[chatId].CongratulateCulture;

            return SupportedLanguages.Default.Culture;
        }
    }
    public bool SetCongratulateCulture(long chatId, string languageLabel)
    {
        lock (_lock)
        {
            if (!_data.Chats.ContainsKey(chatId))
                _data.Chats[chatId] = new ChatData();

            if (!SupportedLanguages.ByLabel.TryGetValue(languageLabel, out var value))
                return false;

            _data.Chats[chatId].CongratulateCulture = value.Culture;

            Save();
            return true;
        }
    }

    private (string? username, string displayName) ParseIdentity(string input)
    {
        input = input.Trim();

        if (input.StartsWith("@"))
            return (input, input);

        return (null, input);
    }

    private string BuildKey(string? username, string displayName)
    {
        return username != null
            ? username.ToLowerInvariant()
            : displayName.ToLowerInvariant();
    }

    private BotData Load()
    {
        if (!File.Exists(_file))
            return new BotData();

        var json = File.ReadAllText(_file);

        return JsonSerializer.Deserialize<BotData>(json) ?? new BotData();
    }

    private void Save()
    {
        File.WriteAllText(_file,
            JsonSerializer.Serialize(_data, new JsonSerializerOptions
            {
                WriteIndented = true
            }));
    }
}
