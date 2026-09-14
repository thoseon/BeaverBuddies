using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Navigation;

namespace Timberborn.Buildings;

internal class BuildingBlockedAccessible : BaseComponent, IAwakableComponent, IBlockedAccessible, IFinishedStateListener
{
	private readonly INavMeshService _navMeshService;

	private BlockObject _blockObject;

	public BuildingBlockedAccessible(INavMeshService navMeshService)
	{
		_navMeshService = navMeshService;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		DisableComponent();
	}

	public bool IsBlocked()
	{
		PositionedEntrance positionedEntrance = _blockObject.PositionedEntrance;
		if (base.Enabled)
		{
			return !_navMeshService.AreConnected(positionedEntrance.Coordinates, positionedEntrance.DoorstepCoordinates);
		}
		return false;
	}

	public bool IsBlockedInstant()
	{
		PositionedEntrance positionedEntrance = _blockObject.PositionedEntrance;
		if (base.Enabled)
		{
			return !_navMeshService.AreConnectedInstant(positionedEntrance.Coordinates, positionedEntrance.DoorstepCoordinates);
		}
		return false;
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
	}

	public void OnExitFinishedState()
	{
		DisableComponent();
	}
}
