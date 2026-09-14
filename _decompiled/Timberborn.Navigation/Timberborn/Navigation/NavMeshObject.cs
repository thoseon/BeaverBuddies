using System.Collections.Generic;
using UnityEngine;

namespace Timberborn.Navigation;

public class NavMeshObject
{
	private readonly NavMeshUpdater _navMeshUpdater;

	private readonly RestrictedNodeUpdater _restrictedNodeUpdater;

	private readonly List<NavMeshChangeSpecification> _addingChanges = new List<NavMeshChangeSpecification>();

	private readonly List<NavMeshChangeSpecification> _removingChanges = new List<NavMeshChangeSpecification>();

	private readonly List<Vector3Int> _restrictedCoordinates = new List<Vector3Int>();

	private bool _addedToPreviewNavMesh;

	internal NavMeshObject(NavMeshUpdater navMeshUpdater, RestrictedNodeUpdater restrictedNodeUpdater)
	{
		_navMeshUpdater = navMeshUpdater;
		_restrictedNodeUpdater = restrictedNodeUpdater;
	}

	public void Reset()
	{
		_addingChanges.Clear();
		_removingChanges.Clear();
		_restrictedCoordinates.Clear();
	}

	public void AddEdge(NavMeshEdge navMeshEdge)
	{
		_addingChanges.Add(new NavMeshChangeSpecification(navMeshEdge, NavMeshChangeType.AddEdge));
		_removingChanges.Add(new NavMeshChangeSpecification(navMeshEdge, NavMeshChangeType.RemoveEdge));
	}

	public void BlockEdge(NavMeshEdge navMeshEdge)
	{
		_addingChanges.Add(new NavMeshChangeSpecification(navMeshEdge, NavMeshChangeType.BlockEdge));
		_removingChanges.Add(new NavMeshChangeSpecification(navMeshEdge, NavMeshChangeType.UnblockEdge));
	}

	public void AddRestrictedCoordinates(Vector3Int coordinates)
	{
		_restrictedCoordinates.Add(coordinates);
	}

	public void EnqueueAddToRegularNavMesh()
	{
		_navMeshUpdater.EnqueueRegularChanges(_addingChanges);
		_restrictedNodeUpdater.EnqueueAddingChange(_restrictedCoordinates);
	}

	public void EnqueueRemoveFromRegularNavMesh()
	{
		_navMeshUpdater.EnqueueRegularChanges(_removingChanges);
		_restrictedNodeUpdater.EnqueueRemovingChange(_restrictedCoordinates);
	}

	public void EnqueueAddToPreviewNavMesh()
	{
		if (!_addedToPreviewNavMesh)
		{
			_navMeshUpdater.EnqueuePreviewChanges(_addingChanges);
			_addedToPreviewNavMesh = true;
		}
	}

	public void EnqueueRemoveFromPreviewNavMesh()
	{
		if (_addedToPreviewNavMesh)
		{
			_navMeshUpdater.EnqueuePreviewChanges(_removingChanges);
			_addedToPreviewNavMesh = false;
		}
	}

	public void AddToPreviewNavMesh()
	{
		if (!_addedToPreviewNavMesh)
		{
			_navMeshUpdater.ApplyPreviewChanges(_addingChanges);
			_addedToPreviewNavMesh = true;
		}
	}

	public void RemoveFromPreviewNavMesh()
	{
		if (_addedToPreviewNavMesh)
		{
			_navMeshUpdater.ApplyPreviewChanges(_removingChanges);
			_addedToPreviewNavMesh = false;
		}
	}
}
