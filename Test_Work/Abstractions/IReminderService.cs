using Test_Work.Entities;

namespace Test_Work.Abstractions;

public interface IReminderService
{
    Task AddReminderAsync(Reminder reminder);

    Task<List<Reminder>> GetActiveRemindersAsync(long chatId);
    
    Task MarkAsDeletedAsync(int reminderId);
    
    Task<Reminder?> GetReminderByIdAsync(int reminderId);
}
