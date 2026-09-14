using System.Collections.Generic;
using Timberborn.BlueprintSystem;
using Timberborn.Common;
using Timberborn.Navigation;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.ZiplineSystem;

public class ZiplineCableNavMesh : ILoadableSingleton
{
	private record EdgePair(NavMeshEdge EdgeFrom, NavMeshEdge EdgeTo);

	private readonly ISpecService _specService;

	private readonly INavMeshService _navMeshService;

	private readonly ZiplineGroupService _ziplineGroupService;

	private readonly Dictionary<CableKey, EdgePair> _regularNavMeshEdges = new Dictionary<CableKey, EdgePair>();

	private readonly Dictionary<CableKey, EdgePair> _previewNavMeshEdges = new Dictionary<CableKey, EdgePair>();

	private float _cableUnitCost;

	public ZiplineCableNavMesh(ISpecService specService, INavMeshService navMeshService, ZiplineGroupService ziplineGroupService)
	{
		_specService = specService;
		_navMeshService = navMeshService;
		_ziplineGroupService = ziplineGroupService;
	}

	public void Load()
	{
		_cableUnitCost = _specService.GetSingleSpec<ZiplineCableNavMeshSpec>().CableUnitCost;
	}

	public void AddInactiveConnection(ZiplineTower ziplineTower, ZiplineTower otherZiplineTower)
	{
		AddPreviewConnectionToNavMesh(ziplineTower, otherZiplineTower);
	}

	public void AddActiveConnection(ZiplineTower ziplineTower, ZiplineTower otherZiplineTower)
	{
		AddPreviewConnectionToNavMesh(ziplineTower, otherZiplineTower);
		AddRegularConnectionToNavMesh(ziplineTower, otherZiplineTower);
	}

	public void ActivateConnection(ZiplineTower ziplineTower, ZiplineTower otherZiplineTower)
	{
		AddRegularConnectionToNavMesh(ziplineTower, otherZiplineTower);
	}

	public void RemoveConnection(ZiplineTower ziplineTower, ZiplineTower otherZiplineTower)
	{
		RemoveConnectionFromNavMesh(ziplineTower, otherZiplineTower);
	}

	private void AddPreviewConnectionToNavMesh(ZiplineTower ziplineTower, ZiplineTower otherZiplineTower)
	{
		if (!TryGetPreviewEdges(ziplineTower, otherZiplineTower, out var _))
		{
			EdgePair edgePair = CreateEdges(ziplineTower, otherZiplineTower);
			_navMeshService.AddPreviewEdge(edgePair.EdgeFrom);
			_navMeshService.AddPreviewEdge(edgePair.EdgeTo);
			_previewNavMeshEdges.Add(CableKey.Create(ziplineTower, otherZiplineTower), edgePair);
		}
	}

	private void AddRegularConnectionToNavMesh(ZiplineTower ziplineTower, ZiplineTower otherZiplineTower)
	{
		if (!TryGetRegularEdges(ziplineTower, otherZiplineTower, out var _))
		{
			EdgePair edgePair = CreateEdges(ziplineTower, otherZiplineTower);
			_navMeshService.AddEdge(edgePair.EdgeFrom);
			_navMeshService.AddEdge(edgePair.EdgeTo);
			_regularNavMeshEdges.Add(CableKey.Create(ziplineTower, otherZiplineTower), edgePair);
		}
	}

	private void RemoveConnectionFromNavMesh(ZiplineTower ziplineTower, ZiplineTower otherZiplineTower)
	{
		if (TryGetPreviewEdges(ziplineTower, otherZiplineTower, out var edges))
		{
			_navMeshService.RemovePreviewEdge(edges.EdgeFrom);
			_navMeshService.RemovePreviewEdge(edges.EdgeTo);
			_previewNavMeshEdges.Remove(CableKey.Create(ziplineTower, otherZiplineTower));
		}
		if (TryGetRegularEdges(ziplineTower, otherZiplineTower, out var edges2))
		{
			_navMeshService.RemoveEdge(edges2.EdgeFrom);
			_navMeshService.RemoveEdge(edges2.EdgeTo);
			_regularNavMeshEdges.Remove(CableKey.Create(ziplineTower, otherZiplineTower));
		}
	}

	private EdgePair CreateEdges(ZiplineTower ziplineTower, ZiplineTower otherZiplineTower)
	{
		Vector3Int vector3Int = ziplineTower.CableAnchorPoint.FloorToInt();
		Vector3Int vector3Int2 = otherZiplineTower.CableAnchorPoint.FloorToInt();
		float cost = _cableUnitCost * (vector3Int - vector3Int2).magnitude;
		int regularGroupId = _ziplineGroupService.RegularGroupId;
		NavMeshEdge edgeFrom = NavMeshEdge.CreateGrouped(vector3Int, vector3Int2, regularGroupId, isRoad: true, cost);
		NavMeshEdge edgeTo = NavMeshEdge.CreateGrouped(vector3Int2, vector3Int, regularGroupId, isRoad: true, cost);
		return new EdgePair(edgeFrom, edgeTo);
	}

	private bool TryGetRegularEdges(ZiplineTower ziplineTower, ZiplineTower otherZiplineTower, out EdgePair edges)
	{
		return _regularNavMeshEdges.TryGetValue(CableKey.Create(ziplineTower, otherZiplineTower), out edges);
	}

	private bool TryGetPreviewEdges(ZiplineTower ziplineTower, ZiplineTower otherZiplineTower, out EdgePair edges)
	{
		return _previewNavMeshEdges.TryGetValue(CableKey.Create(ziplineTower, otherZiplineTower), out edges);
	}
}
