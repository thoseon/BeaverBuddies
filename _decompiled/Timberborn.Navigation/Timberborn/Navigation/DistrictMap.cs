using System;
using System.Collections.Generic;
using Timberborn.Common;

namespace Timberborn.Navigation;

internal class DistrictMap
{
	private readonly RoadNavMeshGraph _roadNavMeshGraph;

	private readonly TerrainNavMeshGraph _terrainNavMeshGraph;

	private readonly DistrictRoadFlowFieldGenerator _districtRoadFlowFieldGenerator;

	private readonly RoadSpillFlowFieldGenerator _roadSpillFlowFieldGenerator;

	private readonly NavigationDistance _navigationDistance;

	private readonly DistrictObstacleService _districtObstacleService;

	private readonly Dictionary<int, District> _districtCenters = new Dictionary<int, District>();

	private readonly Dictionary<District, AccessFlowField> _districtRoadFlowFields = new Dictionary<District, AccessFlowField>();

	private readonly Dictionary<District, RoadSpillFlowField> _districtRoadSpillFlowFields = new Dictionary<District, RoadSpillFlowField>();

	private readonly Dictionary<int, District> _districtsOnRoads = new Dictionary<int, District>();

	private readonly AccessFlowField _emptyRoadFlowField = new AccessFlowField();

	private readonly RoadSpillFlowField _emptyRoadSpillFlowField = new RoadSpillFlowField();

	private bool _anyRoadFlowFieldDirty = true;

	public DistrictMap(RoadNavMeshGraph roadNavMeshGraph, TerrainNavMeshGraph terrainNavMeshGraph, DistrictRoadFlowFieldGenerator districtRoadFlowFieldGenerator, RoadSpillFlowFieldGenerator roadSpillFlowFieldGenerator, NavigationDistance navigationDistance, DistrictObstacleService districtObstacleService)
	{
		_roadNavMeshGraph = roadNavMeshGraph;
		_terrainNavMeshGraph = terrainNavMeshGraph;
		_districtRoadFlowFieldGenerator = districtRoadFlowFieldGenerator;
		_roadSpillFlowFieldGenerator = roadSpillFlowFieldGenerator;
		_navigationDistance = navigationDistance;
		_districtObstacleService = districtObstacleService;
	}

	public void AddDistrictCenter(District district)
	{
		int centerNodeId = district.CenterNodeId;
		if (_districtCenters.ContainsKey(centerNodeId))
		{
			throw new InvalidOperationException($"There's already district center at {centerNodeId}");
		}
		_districtCenters[centerNodeId] = district;
		_districtRoadFlowFields[district] = new AccessFlowField();
		_districtRoadSpillFlowFields[district] = new RoadSpillFlowField();
		_anyRoadFlowFieldDirty = true;
	}

	public void RemoveDistrictCenter(District district)
	{
		_districtCenters.Remove(district.CenterNodeId);
		_districtRoadFlowFields.Remove(district);
		_districtRoadSpillFlowFields.Remove(district);
		_anyRoadFlowFieldDirty = true;
	}

	public bool HasDistrictCenter(District district)
	{
		if (_districtCenters.TryGetValue(district.CenterNodeId, out var value))
		{
			return district == value;
		}
		return false;
	}

	public AccessFlowField GetDistrictRoadFlowFieldByRoadNodeId(int nodeId)
	{
		RecalculateRoadFlowFields();
		if (!_districtsOnRoads.TryGetValue(nodeId, out var value) || !_districtRoadFlowFields.TryGetValue(value, out var value2))
		{
			return _emptyRoadFlowField;
		}
		return value2;
	}

	public RoadSpillFlowField GetDistrictRoadSpillFlowFieldByRoadNodeId(int nodeId)
	{
		RecalculateRoadFlowFields();
		if (!_districtsOnRoads.TryGetValue(nodeId, out var value))
		{
			return _emptyRoadSpillFlowField;
		}
		return GetDistrictRoadSpillFlowField(value);
	}

	public IReadOnlyCollection<int> DistrictCenterNodeIds()
	{
		return _districtCenters.Keys;
	}

	public void OnObstacleChanged()
	{
		foreach (AccessFlowField value in _districtRoadFlowFields.Values)
		{
			value.Clear();
		}
		_anyRoadFlowFieldDirty = true;
	}

	public void OnNavMeshUpdated(NavMeshUpdate navMeshUpdate)
	{
		ReadOnlyList<int> roadNodeIds = navMeshUpdate.RoadNodeIds;
		for (int i = 0; i < roadNodeIds.Count; i++)
		{
			if (_districtsOnRoads.TryGetValue(roadNodeIds[i], out var value))
			{
				ClearDistrictRoadFlowFieldIfNotAlreadyRemoved(value);
				_anyRoadFlowFieldDirty = true;
			}
		}
		ReadOnlyList<int> terrainNodeIds = navMeshUpdate.TerrainNodeIds;
		foreach (RoadSpillFlowField value2 in _districtRoadSpillFlowFields.Values)
		{
			for (int j = 0; j < terrainNodeIds.Count; j++)
			{
				if (value2.HasNode(terrainNodeIds[j]))
				{
					value2.Clear();
					break;
				}
			}
		}
	}

	public bool RoadNodeIsOccupiedByDistrict(District district, int nodeId)
	{
		RecalculateRoadFlowFields();
		if (_districtsOnRoads.TryGetValue(nodeId, out var value))
		{
			return value == district;
		}
		return false;
	}

	public bool NodeHasAnyDistrictRoadSpillFlowField(int nodeId)
	{
		foreach (District value in _districtCenters.Values)
		{
			if (GetDistrictRoadSpillFlowField(value).HasNode(nodeId))
			{
				return true;
			}
		}
		return false;
	}

	public bool TryGetParentRoadNode(District district, int nodeId, out int parentNode)
	{
		RoadSpillFlowField districtRoadSpillFlowField = GetDistrictRoadSpillFlowField(district);
		if (districtRoadSpillFlowField.HasNode(nodeId))
		{
			parentNode = districtRoadSpillFlowField.GetRoadParentNodeId(nodeId);
			return true;
		}
		parentNode = 0;
		return false;
	}

	private void ClearDistrictRoadFlowFieldIfNotAlreadyRemoved(District districtId)
	{
		if (_districtRoadFlowFields.TryGetValue(districtId, out var value))
		{
			value.Clear();
		}
	}

	private void RecalculateRoadFlowFields()
	{
		if (!_anyRoadFlowFieldDirty)
		{
			return;
		}
		_districtsOnRoads.Clear();
		foreach (var (districtCenterNodeId, district2) in _districtCenters)
		{
			AccessFlowField accessFlowField = _districtRoadFlowFields[district2];
			if (!accessFlowField.IsFilled)
			{
				RecalculateRoadFlowField(accessFlowField, districtCenterNodeId);
				_districtRoadSpillFlowFields[district2].Clear();
			}
			AssignDistrictToRoadMap(district2);
		}
		_anyRoadFlowFieldDirty = false;
	}

	private void RecalculateRoadFlowField(AccessFlowField roadFlowField, int districtCenterNodeId)
	{
		_districtRoadFlowFieldGenerator.FillFlowFieldUpToDistance(_roadNavMeshGraph, _districtObstacleService, roadFlowField, districtCenterNodeId);
	}

	private void AssignDistrictToRoadMap(District district)
	{
		foreach (int allNodeId in _districtRoadFlowFields[district].GetAllNodeIds())
		{
			if (_districtsOnRoads.ContainsKey(allNodeId))
			{
				throw new InvalidOperationException($"District {district} conflicts with district {_districtsOnRoads[allNodeId]}");
			}
			_districtsOnRoads[allNodeId] = district;
		}
	}

	private RoadSpillFlowField GetDistrictRoadSpillFlowField(District district)
	{
		if (_districtRoadSpillFlowFields.TryGetValue(district, out var value))
		{
			if (!value.IsFilled)
			{
				RecalculateRoadFlowFields();
				AccessFlowField roadFlowField = _districtRoadFlowFields[district];
				RecalculateRoadSpillFlowField(roadFlowField, value);
			}
			return value;
		}
		return _emptyRoadSpillFlowField;
	}

	private void RecalculateRoadSpillFlowField(AccessFlowField roadFlowField, RoadSpillFlowField roadSpillFlowField)
	{
		_roadSpillFlowFieldGenerator.FillFlowFieldUpToDistance(_terrainNavMeshGraph, roadFlowField, _navigationDistance.DistrictTerrain, roadSpillFlowField);
	}
}
