using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;

namespace Timberborn.LinkedBuildingSystem;

public class LinkedBuilding : BaseComponent, IAwakableComponent, IFinishedStateListener, IUnfinishedStateListener, IDeletableEntity, IPersistentEntity
{
	private static readonly ComponentKey LinkedBuildingKey = new ComponentKey("LinkedBuilding");

	private static readonly PropertyKey<LinkedBuilding> LinkedKey = new PropertyKey<LinkedBuilding>("Linked");

	private readonly IBlockService _blockService;

	private readonly EntityService _entityService;

	private readonly ReferenceSerializer _referenceSerializer;

	private BlockObject _blockObject;

	private LinkedBuilding _linked;

	public bool IsLinked => _linked;

	public event EventHandler<LinkedBuilding> BuildingLinked;

	public LinkedBuilding(IBlockService blockService, EntityService entityService, ReferenceSerializer referenceSerializer)
	{
		_blockService = blockService;
		_entityService = entityService;
		_referenceSerializer = referenceSerializer;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
	}

	public void Save(IEntitySaver entitySaver)
	{
		if (IsLinked)
		{
			entitySaver.GetComponent(LinkedBuildingKey).Set(LinkedKey, _linked, _referenceSerializer.Of<LinkedBuilding>());
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(LinkedBuildingKey, out var objectLoader))
		{
			LinkBuilding(objectLoader.Get(LinkedKey, _referenceSerializer.Of<LinkedBuilding>()));
		}
	}

	public void OnEnterFinishedState()
	{
		LinkIfNeeded();
	}

	public void OnExitFinishedState()
	{
	}

	public void OnEnterUnfinishedState()
	{
		LinkIfNeeded();
	}

	public void OnExitUnfinishedState()
	{
	}

	public void DeleteEntity()
	{
		if (IsLinked)
		{
			_linked.UnlinkAndDelete();
		}
	}

	private void LinkIfNeeded()
	{
		if (IsLinked)
		{
			return;
		}
		LinkedBuilding bottomObjectComponentAt = _blockService.GetBottomObjectComponentAt<LinkedBuilding>(_blockObject.CoordinatesBehind());
		if ((bool)bottomObjectComponentAt)
		{
			if (bottomObjectComponentAt.IsLinked)
			{
				throw new NotSupportedException("Trying to link to already linked building");
			}
			LinkBuilding(bottomObjectComponentAt);
			bottomObjectComponentAt.LinkBuilding(this);
		}
	}

	private void LinkBuilding(LinkedBuilding linked)
	{
		_linked = linked;
		BuildingLinked?.Invoke(this, linked);
	}

	private void UnlinkAndDelete()
	{
		_linked = null;
		_entityService.Delete(this);
	}
}
