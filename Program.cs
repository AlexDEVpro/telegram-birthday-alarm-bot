using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using TelegramBirthdayAlarmBot.Configuration;
using TelegramBirthdayAlarmBot.Services;
using TelegramBirthdayAlarmBot.Services.Localization;

class Program
{
    static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.Configuration
            .AddJsonFile("appsettings.json", optional: false)
            .AddUserSecrets<Program>()
            .AddEnvironmentVariables();
        builder.Services.Configure<TelegramOptions>(
            builder.Configuration.GetSection(nameof(TelegramOptions)));
        builder.Services.Configure<BirthdayOptions>(
            builder.Configuration.GetSection(nameof(BirthdayOptions)));

        builder.Services.AddSingleton<ITelegramBotClient>(sp =>
        {
            var botToken = sp.GetRequiredService<IOptions<TelegramOptions>>().Value.BotToken;

            return new TelegramBotClient(botToken);
        });

        builder.Services.AddSingleton<BotTimeProvider>();
        builder.Services.AddSingleton<StorageService>();
        builder.Services.AddSingleton<BotService>();
        builder.Services.AddSingleton<PendingAddStateService>();
        builder.Services.AddSingleton<UserCultureResolver>();

        builder.Services.AddHostedService<SetBotCommandsService>();
        builder.Services.AddHostedService<BirthdayService>();

        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
        });

        var host = builder.Build();

        var botService = host.Services.GetRequiredService<BotService>();
        botService.Start();

        Console.WriteLine("Bot running...");

        await host.RunAsync();
    }
}