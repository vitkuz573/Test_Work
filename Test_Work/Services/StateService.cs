using System.Collections.Concurrent;
using Test_Work.Abstractions;

namespace Test_Work.Services;

public class StateService : IStateService
{
    public ConcurrentDictionary<long, int> ReminderSteps { get; } = new();

    public HashSet<long> PendingDeletionReminders { get; } = [];

    public ConcurrentDictionary<long, bool> PendingCalculations { get; } = new();
}
