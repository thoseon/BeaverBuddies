using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.BuildingRange;
using Timberborn.Buildings;
using Timberborn.Navigation;
using UnityEngine;

namespace Timberborn.BuildingsNavigation;

internal class BuildingCachingFlowField : BaseComponent, IAwakableComponent, IFinishedStateListener
{
	private readonly INavigationCachingService _navigationCachingService;

	private BuildingAccessible _buildingAccessible;

	private BuildingWithTerrainRange _buildingWithTerrainRange;

	private Vector3Int _accessCoordinates;

	public BuildingCachingFlowField(INavigationCachingService navigationCachingService)
	{
		_navigationCachingService = navigationCachingService;
	}

	public void Awake()
	{
		_buildingAccessible = GetComponent<BuildingAccessible>();
		_buildingWithTerrainRange = GetComponent<BuildingWithTerrainRange>();
	}

	public void OnEnterFinishedState()
	{
		StartCaching();
	}

	public void OnExitFinishedState()
	{
		StopCaching();
	}

	private void StartCaching()
	{
		_accessCoordinates = NavigationCoordinateSystem.WorldToGridInt(_buildingAccessible.Accessible.Accesses.Single());
		_navigationCachingService.StartCachingRoadFlowField(_accessCoordinates);
		if ((bool)_buildingWithTerrainRange)
		{
			_navigationCachingService.StartCachingTerrainFlowField(_accessCoordinates);
		}
	}

	private void StopCaching()
	{
		_navigationCachingService.StopCachingRoadFlowField(_accessCoordinates);
		if ((bool)_buildingWithTerrainRange)
		{
			_navigationCachingService.StopCachingTerrainFlowField(_accessCoordinates);
		}
	}
}
