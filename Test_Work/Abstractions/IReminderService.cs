using Test_Work.Entities;

namespace Test_Work.Abstractions;

public interface IReminderService
{
    Task AddReminderAsync(Reminder reminder);
    
    Task<List<Reminder>> GetActiveRemindersAsync(long chatId);
    
    Task<Reminder?> GetReminderByIdAsync(int reminderId);
    
    Task MarkAsDeletedAsync(int reminderId);
    
    Task<List<Reminder>> GetDueRemindersAsync();
    
    Task MarkAsSentAsync(int reminderId);
}
