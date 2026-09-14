using System;
using System.Collections.Generic;

namespace Timberborn.TickSystem;

internal class TickableEntityBucket
{
	private readonly SortedList<Guid, TickableEntity> _tickableEntities = new SortedList<Guid, TickableEntity>();

	private readonly List<TickableEntity> _entitiesToRemove = new List<TickableEntity>();

	private bool _isTicking;

	public void Add(TickableEntity tickableEntity)
	{
		_tickableEntities.Add(tickableEntity.EntityId, tickableEntity);
	}

	public void Remove(TickableEntity tickableEntity)
	{
		if (_isTicking)
		{
			_entitiesToRemove.Add(tickableEntity);
		}
		else
		{
			_tickableEntities.Remove(tickableEntity.EntityId);
		}
	}

	public void TickAll()
	{
		_isTicking = true;
		for (int i = 0; i < _tickableEntities.Count; i++)
		{
			_tickableEntities.Values[i].Tick();
		}
		_isTicking = false;
		for (int j = 0; j < _entitiesToRemove.Count; j++)
		{
			_tickableEntities.Remove(_entitiesToRemove[j].EntityId);
		}
		_entitiesToRemove.Clear();
	}
}
