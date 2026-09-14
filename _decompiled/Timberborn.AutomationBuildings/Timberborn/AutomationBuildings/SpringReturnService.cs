using System.Collections.Generic;
using Timberborn.Automation;

namespace Timberborn.AutomationBuildings;

internal class SpringReturnService : ICommittingSingleton
{
	private readonly List<Lever> _levers = new List<Lever>();

	public void CommitTick()
	{
		for (int i = 0; i < _levers.Count; i++)
		{
			if ((bool)_levers[i])
			{
				_levers[i].SpringReturnToOff();
			}
		}
		_levers.Clear();
	}

	public void Register(Lever lever)
	{
		_levers.Add(lever);
	}
}
