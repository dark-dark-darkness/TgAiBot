using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using TgAiBog.Contract;

namespace TgAiBog.Core.Extensions;

public static class DependencyInjection
{
    public static void AddTelegramBot(this IServiceCollection services, string connectionName)
    {
        services.AddSingleton(sp => new TelegramBotClient(
            sp.GetRequiredService<IConfiguration>().GetConnectionString(connectionName)!));
        services.AddHostedService<TelegramBotConfigureService>();
    }
}

file class TelegramBotConfigureService(
    TelegramBotClient bot,
    ILogger<TelegramBotClient> logger,
    IServiceProvider serviceProvider) : BackgroundService
{
    override protected async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await bot.DropPendingUpdatesAsync(stoppingToken);

        bot.OnError += async (ex, source) =>
            await PublishMessageAsync(new TelegramErrorEvent(Guid.NewGuid(), ex, source), bot.GlobalCancelToken);
        bot.OnMessage += async (msg, type) =>
            await PublishMessageAsync(new TelegramMessageEvent(Guid.NewGuid(), msg, type), bot.GlobalCancelToken);
        bot.OnUpdate += async (update) =>
            await PublishMessageAsync(new TelegramUpdateEvent(Guid.NewGuid(), update), bot.GlobalCancelToken);
    }

    private async Task PublishMessageAsync<T>(T msg, CancellationToken cancellationToken) where T : class
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var bus = scope.ServiceProvider.GetRequiredService<IBus>();
        await bus.Publish(msg, cancellationToken);
    }
}