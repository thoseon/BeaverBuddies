using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Buildings;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.Navigation;
using UnityEngine;

namespace Timberborn.BuildingsNavigation;

public class BuildingTerrainRange : BaseComponent, IAwakableComponent, IFinishedStateListener, INavMeshListener
{
	private readonly INavigationRangeService _navigationRangeService;

	private readonly INavMeshListenerEntityRegistry _navMeshListenerEntityRegistry;

	private readonly NavigationDistance _navigationDistance;

	private BuildingAccessible _buildingAccessible;

	private readonly HashSet<Vector3Int> _range = new HashSet<Vector3Int>();

	private BoundingBox _boundingBox;

	private bool _dirty;

	private bool _rangeInitialized;

	private Vector3? Access => _buildingAccessible.Accessible.UnblockedSingleAccess;

	public event EventHandler<RangeChangedEventArgs> RangeChanged;

	public BuildingTerrainRange(INavigationRangeService navigationRangeService, INavMeshListenerEntityRegistry navMeshListenerEntityRegistry, NavigationDistance navigationDistance)
	{
		_navigationRangeService = navigationRangeService;
		_navMeshListenerEntityRegistry = navMeshListenerEntityRegistry;
		_navigationDistance = navigationDistance;
	}

	public void Awake()
	{
		_buildingAccessible = GetComponent<BuildingAccessible>();
	}

	public ReadOnlyHashSet<Vector3Int> GetRange()
	{
		if (_dirty)
		{
			UpdateRange();
		}
		return _range.AsReadOnlyHashSet();
	}

	public void OnNavMeshUpdated(NavMeshUpdate navMeshUpdate)
	{
		if (_boundingBox.Intersects(navMeshUpdate.Bounds))
		{
			_dirty = true;
			RangeChanged?.Invoke(this, new RangeChangedEventArgs(!_rangeInitialized));
			_rangeInitialized = true;
		}
	}

	public void OnEnterFinishedState()
	{
		_navMeshListenerEntityRegistry.RegisterNavMeshListener(this);
		UpdateBoundingBox();
		_dirty = true;
	}

	public void OnExitFinishedState()
	{
		_navMeshListenerEntityRegistry.UnregisterNavMeshListener(this);
	}

	private void UpdateRange()
	{
		_range.Clear();
		if (Access.HasValue)
		{
			_range.AddRange(_navigationRangeService.GetTerrainNodesInRange(Access.Value));
			_dirty = false;
		}
	}

	private void UpdateBoundingBox()
	{
		Vector3 vector = CoordinateSystem.GridToWorld(_buildingAccessible.Accessible.Accesses.Single());
		float num = _navigationDistance.ResourceBuildings + 2f;
		Vector3 vector2 = new Vector3(num, num, num);
		Vector3 value = vector + vector2;
		Vector3 value2 = vector - vector2;
		BoundingBox.Builder builder = default(BoundingBox.Builder);
		builder.Expand(value.CeilToInt());
		builder.Expand(value2.FloorToInt());
		_boundingBox = builder.Build();
	}
}
