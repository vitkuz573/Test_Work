using Microsoft.EntityFrameworkCore;
using Test_Work.Configurations;
using Test_Work.Entities;

namespace Test_Work.Data;

public class ReminderDbContext(DbContextOptions<ReminderDbContext> options) : DbContext(options)
{
    public DbSet<Reminder> Reminders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ReminderConfiguration());
    }
}
