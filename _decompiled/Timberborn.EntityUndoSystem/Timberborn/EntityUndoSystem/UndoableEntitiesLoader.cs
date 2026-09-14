using System.Collections.Generic;
using Timberborn.UndoSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.EntityUndoSystem;

public class UndoableEntitiesLoader : IUndoPostprocessor
{
	private readonly EntitiesLoader _entitiesLoader;

	private readonly List<InstantiatedSerializedEntity> _entitiesToLoad = new List<InstantiatedSerializedEntity>();

	public UndoableEntitiesLoader(EntitiesLoader entitiesLoader)
	{
		_entitiesLoader = entitiesLoader;
	}

	public void AddEntityForLoad(InstantiatedSerializedEntity entity)
	{
		_entitiesToLoad.Add(entity);
	}

	public void Reload(InstantiatedSerializedEntity entity)
	{
		_entitiesLoader.Load(new InstantiatedSerializedEntity[1] { entity });
	}

	public void PostprocessUndoables()
	{
		if (_entitiesToLoad.Count > 0)
		{
			_entitiesLoader.LoadAndInitialize(_entitiesToLoad);
			_entitiesLoader.PostLoad(_entitiesToLoad);
			_entitiesToLoad.Clear();
		}
	}
}
