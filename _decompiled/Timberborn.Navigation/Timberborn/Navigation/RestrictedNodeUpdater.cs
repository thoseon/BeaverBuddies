using System.Collections.Generic;
using Timberborn.Common;
using UnityEngine;

namespace Timberborn.Navigation;

internal class RestrictedNodeUpdater
{
	private readonly struct RestrictedNodeChange
	{
		public int NodeId { get; }

		public bool AddingChange { get; }

		public RestrictedNodeChange(int nodeId, bool addingChange)
		{
			NodeId = nodeId;
			AddingChange = addingChange;
		}
	}

	private readonly RestrictedNodeMap _restrictedNodeMap;

	private readonly NodeIdService _nodeIdService;

	private readonly Queue<RestrictedNodeChange> _enqueuedChanges = new Queue<RestrictedNodeChange>();

	public RestrictedNodeUpdater(RestrictedNodeMap restrictedNodeMap, NodeIdService nodeIdService)
	{
		_restrictedNodeMap = restrictedNodeMap;
		_nodeIdService = nodeIdService;
	}

	public void EnqueueAddingChange(IReadOnlyCollection<Vector3Int> coordinates)
	{
		foreach (Vector3Int coordinate in coordinates)
		{
			int nodeId = _nodeIdService.GridToId(coordinate);
			_enqueuedChanges.Enqueue(new RestrictedNodeChange(nodeId, addingChange: true));
		}
	}

	public void EnqueueRemovingChange(IReadOnlyCollection<Vector3Int> coordinates)
	{
		foreach (Vector3Int coordinate in coordinates)
		{
			int nodeId = _nodeIdService.GridToId(coordinate);
			_enqueuedChanges.Enqueue(new RestrictedNodeChange(nodeId, addingChange: false));
		}
	}

	public void ProcessRegularChanges()
	{
		while (!_enqueuedChanges.IsEmpty())
		{
			ProcessChange();
		}
	}

	private void ProcessChange()
	{
		RestrictedNodeChange restrictedNodeChange = _enqueuedChanges.Dequeue();
		if (restrictedNodeChange.AddingChange)
		{
			_restrictedNodeMap.RestrictNode(restrictedNodeChange.NodeId);
		}
		else
		{
			_restrictedNodeMap.UnrestrictNode(restrictedNodeChange.NodeId);
		}
	}
}
