using Timberborn.BaseComponentSystem;
using Timberborn.BuildingsNavigation;
using Timberborn.EntitySystem;
using Timberborn.Navigation;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.AutomationBuildings;

internal class GateNavMeshBlocker : BaseComponent, IAwakableComponent, IPreInitializableEntity, IPersistentEntity
{
	private static readonly ComponentKey ComponentKey = new ComponentKey("GateNavMeshBlocker");

	private static readonly PropertyKey<bool> NavMeshBlockedKey = new PropertyKey<bool>("NavMeshBlocked");

	private readonly INavMeshService _navMeshService;

	private readonly NavMeshGroupService _navMeshGroupService;

	private GatePlacement _gatePlacement;

	private BuildingNavMesh _buildingNavMesh;

	private bool _expensiveTraverseCostSet;

	public bool NavMeshBlocked { get; private set; }

	public GateNavMeshBlocker(INavMeshService navMeshService, NavMeshGroupService navMeshGroupService)
	{
		_navMeshService = navMeshService;
		_navMeshGroupService = navMeshGroupService;
	}

	public void Awake()
	{
		_gatePlacement = GetComponent<GatePlacement>();
		_buildingNavMesh = GetComponent<BuildingNavMesh>();
	}

	public void PreInitializeEntity()
	{
		if (NavMeshBlocked)
		{
			Block();
		}
	}

	public void Save(IEntitySaver entitySaver)
	{
		entitySaver.GetComponent(ComponentKey).Set(NavMeshBlockedKey, NavMeshBlocked);
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(ComponentKey);
		NavMeshBlocked = component.Get(NavMeshBlockedKey);
	}

	public void Block()
	{
		SetPathBlockage(isBlocked: true);
		SetTraverseCost(isExpensive: true);
		NavMeshBlocked = true;
	}

	public void Unblock()
	{
		SetPathBlockage(isBlocked: false);
		SetTraverseCost(isExpensive: false);
		NavMeshBlocked = false;
	}

	private void SetPathBlockage(bool isBlocked)
	{
		if (isBlocked)
		{
			_buildingNavMesh.BlockAndRemoveFromNavMesh();
		}
		else
		{
			_buildingNavMesh.UnblockAndAddToNavMesh();
		}
	}

	private void SetTraverseCost(bool isExpensive)
	{
		if (isExpensive != _expensiveTraverseCostSet)
		{
			Vector3Int start = _gatePlacement.Start;
			Vector3Int end = _gatePlacement.End;
			Vector3Int center = _gatePlacement.Center;
			if (isExpensive)
			{
				_navMeshService.RemoveEdge(GetNormalEdge(center, start));
				_navMeshService.RemoveEdge(GetNormalEdge(center, end));
				_navMeshService.AddEdge(GetExpensiveEdge(center, start));
				_navMeshService.AddEdge(GetExpensiveEdge(center, end));
			}
			else
			{
				_navMeshService.AddEdge(GetNormalEdge(center, start));
				_navMeshService.AddEdge(GetNormalEdge(center, end));
				_navMeshService.RemoveEdge(GetExpensiveEdge(center, start));
				_navMeshService.RemoveEdge(GetExpensiveEdge(center, end));
			}
			_expensiveTraverseCostSet = isExpensive;
		}
	}

	private NavMeshEdge GetNormalEdge(Vector3Int start, Vector3Int end)
	{
		return GetEdge(start, end, 1f);
	}

	private NavMeshEdge GetExpensiveEdge(Vector3Int start, Vector3Int end)
	{
		return GetEdge(start, end, NavigationLimits.MaxEdgeCost);
	}

	private NavMeshEdge GetEdge(Vector3Int start, Vector3Int end, float cost)
	{
		return NavMeshEdge.CreateGrouped(start, end, _navMeshGroupService.GetDefaultGroupId(), isRoad: false, cost);
	}
}
