using Telegram.Bot;
using Test_Work.Abstractions;

namespace Test_Work.Services;

public class TelegramBotHostedService(ITelegramBotClient botClient, IServiceScopeFactory scopeFactory) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        var scope = scopeFactory.CreateScope();
        var botService = scope.ServiceProvider.GetRequiredService<IBotService>();

        botClient.StartReceiving(
            async (_, update, _) =>
            {
                await botService.HandleUpdateAsync(update);
            },
            async (_, exception, _) =>
            {
                Console.WriteLine($"Ошибка: {exception.Message}");
                await Task.CompletedTask;
            },
            cancellationToken: cancellationToken
        );

        Console.WriteLine("Telegram бот запущен...");

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Telegram бот остановлен...");

        return Task.CompletedTask;
    }
}
