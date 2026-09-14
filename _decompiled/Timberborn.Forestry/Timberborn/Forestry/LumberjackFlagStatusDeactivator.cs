using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.BuildingsNavigation;
using Timberborn.SingletonSystem;
using Timberborn.WorkSystem;
using UnityEngine;

namespace Timberborn.Forestry;

internal class LumberjackFlagStatusDeactivator : BaseComponent, IAwakableComponent, IFinishedStateListener
{
	private readonly EventBus _eventBus;

	private readonly TreeCuttingArea _treeCuttingArea;

	private NothingToDoInRangeStatus _nothingToDoInRangeStatus;

	private BuildingTerrainRange _buildingTerrainRange;

	public LumberjackFlagStatusDeactivator(EventBus eventBus, TreeCuttingArea treeCuttingArea)
	{
		_eventBus = eventBus;
		_treeCuttingArea = treeCuttingArea;
	}

	public void Awake()
	{
		_nothingToDoInRangeStatus = GetComponent<NothingToDoInRangeStatus>();
		_buildingTerrainRange = GetComponent<BuildingTerrainRange>();
	}

	public void OnEnterFinishedState()
	{
		_eventBus.Register(this);
	}

	public void OnExitFinishedState()
	{
		_eventBus.Unregister(this);
	}

	[OnEvent]
	public void OnTreeCuttingAreaChanged(TreeCuttingAreaChangedEvent treeCuttingAreaChangedEvent)
	{
		if (treeCuttingAreaChangedEvent.CoordinatesAdded)
		{
			UpdateStatus();
		}
	}

	private void UpdateStatus()
	{
		if (AnyYielderInRange())
		{
			_nothingToDoInRangeStatus.DeactivateStatus();
		}
	}

	private bool AnyYielderInRange()
	{
		foreach (Vector3Int item in _buildingTerrainRange.GetRange())
		{
			if (_treeCuttingArea.HasYielder(item))
			{
				return true;
			}
		}
		return false;
	}
}
