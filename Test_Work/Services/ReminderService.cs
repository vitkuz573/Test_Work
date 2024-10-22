using Microsoft.EntityFrameworkCore;
using Test_Work.Abstractions;
using Test_Work.Data;
using Test_Work.Entities;

namespace Test_Work.Services;

public class ReminderService(ReminderDbContext dbContext) : IReminderService
{
    public async Task AddReminderAsync(Reminder reminder)
    {
        await dbContext.Reminders.AddAsync(reminder);
        await dbContext.SaveChangesAsync();
    }

    public async Task<List<Reminder>> GetActiveRemindersAsync(long chatId)
    {
        return await dbContext.Reminders
            .Where(r => !r.IsDeleted && r.ReminderDate > DateTime.Now && r.ChatId == chatId)
            .ToListAsync();
    }

    public async Task MarkAsDeletedAsync(int reminderId)
    {
        var reminder = await dbContext.Reminders.FindAsync(reminderId);

        if (reminder != null)
        {
            reminder.IsDeleted = true;

            await dbContext.SaveChangesAsync();
        }
    }

    public async Task<Reminder?> GetReminderByIdAsync(int reminderId)
    {
        return await dbContext.Reminders.FindAsync(reminderId);
    }

    public async Task<List<Reminder>> GetDueRemindersAsync()
    {
        return await dbContext.Reminders
            .Where(r => !r.IsDeleted && r.ReminderDate <= DateTime.Now && !r.IsSent)
            .ToListAsync();
    }

    public async Task MarkAsSentAsync(int reminderId)
    {
        var reminder = await dbContext.Reminders.FindAsync(reminderId);

        if (reminder != null)
        {
            reminder.IsSent = true;
            await dbContext.SaveChangesAsync();
        }
    }
}
