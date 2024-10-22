using System.Collections.Concurrent;

namespace Test_Work.Abstractions;

public interface IStateService
{
    ConcurrentDictionary<long, int> ReminderSteps { get; }

    HashSet<long> PendingDeletionReminders { get; }

    ConcurrentDictionary<long, bool> PendingCalculations { get; }
}
