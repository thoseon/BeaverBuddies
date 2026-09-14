using System.Collections.Generic;
using System.Collections.ObjectModel;
using Timberborn.PrioritySystem;
using Timberborn.SingletonSystem;

namespace Timberborn.RecoveredGoodSystem;

internal class PrioritizedRecoveredGoodStackRegistry : ILoadableSingleton
{
	private readonly Dictionary<Priority, SortedList<int, RecoveredGoodStack>> _recoveredGoodStacks = new Dictionary<Priority, SortedList<int, RecoveredGoodStack>>();

	private readonly Dictionary<Priority, ReadOnlyCollection<RecoveredGoodStack>> _recoveredGoodStacksAsReadOnly = new Dictionary<Priority, ReadOnlyCollection<RecoveredGoodStack>>();

	public ReadOnlyCollection<RecoveredGoodStack> GetRecoveredGoodStacks(Priority priority)
	{
		return _recoveredGoodStacksAsReadOnly[priority];
	}

	public void Load()
	{
		foreach (Priority item in Priorities.Ascending)
		{
			SortedList<int, RecoveredGoodStack> sortedList = new SortedList<int, RecoveredGoodStack>();
			_recoveredGoodStacks[item] = sortedList;
			_recoveredGoodStacksAsReadOnly[item] = new ReadOnlyCollection<RecoveredGoodStack>(sortedList.Values);
		}
	}

	public void AddStack(RecoveredGoodStack recoveredGoodStack, Priority priority, int order)
	{
		_recoveredGoodStacks[priority].Add(order, recoveredGoodStack);
	}

	public void RemoveStack(Priority priority, int order)
	{
		_recoveredGoodStacks[priority].Remove(order);
	}
}
