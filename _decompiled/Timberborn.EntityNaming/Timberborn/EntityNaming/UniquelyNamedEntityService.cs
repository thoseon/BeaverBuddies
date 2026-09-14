using System;
using System.Collections.Generic;
using Timberborn.Common;

namespace Timberborn.EntityNaming;

public class UniquelyNamedEntityService
{
	private readonly Dictionary<string, List<UniquelyNamedEntity>> _entitiesByName = new Dictionary<string, List<UniquelyNamedEntity>>();

	public bool TryGet(string name, out UniquelyNamedEntity uniquelyNamedEntity)
	{
		if (_entitiesByName.TryGetValue(name, out var value) && value.Count == 1)
		{
			uniquelyNamedEntity = value[0];
			return true;
		}
		uniquelyNamedEntity = null;
		return false;
	}

	internal void RegisterName(string name, UniquelyNamedEntity newEntity)
	{
		List<UniquelyNamedEntity> orAdd = _entitiesByName.GetOrAdd(name);
		foreach (UniquelyNamedEntity item in orAdd)
		{
			item.SetNonUnique();
		}
		orAdd.Add(newEntity);
		if (orAdd.Count == 1)
		{
			newEntity.SetUnique();
		}
		else
		{
			newEntity.SetNonUnique();
		}
	}

	internal void UnregisterName(string name, UniquelyNamedEntity removedEntity)
	{
		if (!_entitiesByName.TryGetValue(name, out var value) || !value.Remove(removedEntity))
		{
			throw new InvalidOperationException("Entity '" + removedEntity.Name + "' is not registered under name '" + name + "'");
		}
		if (value.Count == 1)
		{
			value[0].SetUnique();
		}
		else if (value.Count == 0)
		{
			_entitiesByName.Remove(name);
		}
	}
}
