using Telegram.Bot;
using Test_Work.Abstractions;

namespace Test_Work.Services;

public class ReminderHostedService(ITelegramBotClient botClient, IServiceScopeFactory scopeFactory) : IHostedService
{
    private Timer? _timer;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(async _ => await SendRemindersAsync(), null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

        return Task.CompletedTask;
    }

    private async Task SendRemindersAsync()
    {
        var scope = scopeFactory.CreateScope();
        var reminderService = scope.ServiceProvider.GetRequiredService<IReminderService>();

        var reminders = await reminderService.GetDueRemindersAsync();

        foreach (var reminder in reminders)
        {
            await botClient.SendTextMessageAsync(reminder.ChatId, $"Напоминание: {reminder.Description}");
            await reminderService.MarkAsSentAsync(reminder.Id);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }
}
