namespace Test_Work.Entities;

public class Reminder
{
    public int Id { get; set; }

    public long ChatId { get; set; }

    public DateTime ReminderDate { get; set; }

    public string Description { get; set; }

    public bool IsDeleted { get; set; }

    public bool IsSent { get; set; }
}
