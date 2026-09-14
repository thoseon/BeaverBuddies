using System;
using System.Collections.Generic;

namespace Timberborn.DwellingSystem;

internal class StaleAssignableDwellingService
{
	private readonly LinkedList<AutoAssignableDwelling> _dwellings = new LinkedList<AutoAssignableDwelling>();

	public void SetAsStalest(AutoAssignableDwelling autoAssignableDwelling)
	{
		if (_dwellings.Last.Value != autoAssignableDwelling)
		{
			throw new ArgumentException(string.Format("Provided {0} {1} is not last!", "AutoAssignableDwelling", autoAssignableDwelling));
		}
		_dwellings.RemoveLast();
		_dwellings.AddFirst(autoAssignableDwelling);
	}

	public AutoAssignableDwelling GetStalest()
	{
		int count = _dwellings.Count;
		for (int i = 0; i < count; i++)
		{
			AutoAssignableDwelling value = _dwellings.First.Value;
			_dwellings.RemoveFirst();
			RegisterDwelling(value);
			if (value.HasAssignableSlot)
			{
				return value;
			}
		}
		return null;
	}

	public void RegisterDwelling(AutoAssignableDwelling autoAssignableDwelling)
	{
		_dwellings.AddLast(autoAssignableDwelling);
	}

	public void UnregisterDwelling(AutoAssignableDwelling autoAssignableDwelling)
	{
		_dwellings.Remove(autoAssignableDwelling);
	}
}
