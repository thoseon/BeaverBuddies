using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;

namespace Timberborn.ConstructionSites;

internal class ConstructionSiteReservations : BaseComponent
{
	private int _capacity = 1;

	private readonly HashSet<Builder> _builders = new HashSet<Builder>();

	public bool HasFreeSpots => _builders.Count < _capacity;

	public void SetCapacity(int capacity)
	{
		_capacity = capacity;
	}

	public void Reserve(Builder builder)
	{
		if (!_builders.Contains(builder) && !HasFreeSpots)
		{
			throw new InvalidOperationException("Error while assigning builder " + builder.Name + ": construction site is already full.");
		}
		_builders.Add(builder);
	}

	public void Unreserve(Builder builder)
	{
		if (!_builders.Contains(builder))
		{
			throw new InvalidOperationException("Builder " + builder.Name + " did not reserve this construction site.");
		}
		_builders.Remove(builder);
	}
}
