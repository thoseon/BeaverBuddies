using Timberborn.BaseComponentSystem;
using Timberborn.BlockObjectAccesses;
using Timberborn.BlockSystem;
using Timberborn.BuildingsReachability;
using Timberborn.EntitySystem;
using Timberborn.Navigation;
using UnityEngine;

namespace Timberborn.Demolishing;

public class ReachableDemolishable : BaseComponent, IAwakableComponent, IInitializableEntity, IUnreachableEntity
{
	private readonly IDistrictService _districtService;

	private Accessible _accessible;

	private BlockObjectCenter _blockObjectCenter;

	private Demolishable _demolishable;

	public ReachableDemolishable(IDistrictService districtService)
	{
		_districtService = districtService;
	}

	public void Awake()
	{
		_blockObjectCenter = GetComponent<BlockObjectCenter>();
		_demolishable = GetComponent<Demolishable>();
	}

	public void InitializeEntity()
	{
		BlockObjectAccessible component = GetComponent<BlockObjectAccessible>();
		if (component != null)
		{
			_accessible = component.Accessible;
		}
	}

	public bool IsUnreachable()
	{
		if (!_demolishable.IsMarked)
		{
			return false;
		}
		return !IsReachable();
	}

	public bool IsReachable(Accessible start, out float distance)
	{
		if ((bool)_accessible && start.FindRoadToTerrainPath(_accessible, out var _, out distance))
		{
			return true;
		}
		Vector3 worldCenterGrounded = _blockObjectCenter.WorldCenterGrounded;
		if (start.FindRoadToTerrainPath(worldCenterGrounded, out distance))
		{
			return true;
		}
		distance = float.MaxValue;
		return false;
	}

	private bool IsReachable()
	{
		if ((bool)_accessible)
		{
			return _districtService.IsOnInstantDistrictRoadSpill(_accessible);
		}
		return _districtService.IsOnInstantDistrictRoadSpill(_blockObjectCenter.WorldCenterGrounded);
	}
}
