using System.Collections.Generic;
using Timberborn.Navigation;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.AutomationBuildings;

internal class GateUpdater : ILateUpdatableSingleton, ISingletonNavMeshListener
{
	private readonly GateConflictDetector _gateConflictDetector;

	private readonly HashSet<Gate> _gatesScheduledToOpen = new HashSet<Gate>();

	private readonly HashSet<Gate> _gatesScheduledToClose = new HashSet<Gate>();

	private readonly HashSet<Gate> _gatesWithConflict = new HashSet<Gate>();

	private readonly List<Gate> _gatesWithConflictCache = new List<Gate>();

	private readonly Dictionary<Vector3Int, Vector3Int> _openGateCrossings = new Dictionary<Vector3Int, Vector3Int>();

	private bool _hasScheduledGates;

	private bool _hasScheduledUnblocking;

	public GateUpdater(GateConflictDetector gateConflictDetector)
	{
		_gateConflictDetector = gateConflictDetector;
	}

	public void LateUpdateSingleton()
	{
		if (_hasScheduledGates)
		{
			CloseScheduledGates();
			OpenScheduledGates();
			_hasScheduledUnblocking = true;
			_hasScheduledGates = false;
		}
		if (_hasScheduledUnblocking)
		{
			TryOpenConflictedGates();
			_hasScheduledUnblocking = false;
		}
		_openGateCrossings.Clear();
	}

	public void OnNavMeshUpdated(NavMeshUpdate navMeshUpdate)
	{
		_hasScheduledUnblocking = true;
	}

	public void ScheduleToOpen(Gate gate)
	{
		_gatesScheduledToOpen.Add(gate);
		_gatesScheduledToClose.Remove(gate);
		_hasScheduledGates = true;
	}

	public void ScheduleToClose(Gate gate)
	{
		_gatesScheduledToClose.Add(gate);
		_gatesScheduledToOpen.Remove(gate);
		_hasScheduledGates = true;
	}

	public void Remove(Gate gate)
	{
		_gatesScheduledToClose.Remove(gate);
		_gatesScheduledToOpen.Remove(gate);
		RemoveGateFromConflicted(gate);
	}

	private void CloseScheduledGates()
	{
		foreach (Gate item in _gatesScheduledToClose)
		{
			TryCloseGate(item);
		}
		_gatesScheduledToClose.Clear();
	}

	private void OpenScheduledGates()
	{
		foreach (Gate item in _gatesScheduledToOpen)
		{
			TryOpenGate(item);
		}
		_gatesScheduledToOpen.Clear();
	}

	private void TryCloseGate(Gate gate)
	{
		if (!gate.GetComponent<GateNavMeshBlocker>().NavMeshBlocked)
		{
			gate.BlockNavMesh();
		}
		RemoveGateFromConflicted(gate);
	}

	private void TryOpenGate(Gate gate)
	{
		GateNavMeshBlocker component = gate.GetComponent<GateNavMeshBlocker>();
		if (!component.NavMeshBlocked)
		{
			return;
		}
		GatePlacement component2 = gate.GetComponent<GatePlacement>();
		if (_gateConflictDetector.CanOpenGateWithoutConflict(component2.Start, component2.End, component2.Center, _openGateCrossings))
		{
			gate.UnblockNavMesh();
			RemoveGateFromConflicted(gate);
			AddToOpenGateCrossings(component2);
			return;
		}
		if (!component.NavMeshBlocked)
		{
			gate.BlockNavMesh();
		}
		AddGateToConflicted(gate);
	}

	private void AddGateToConflicted(Gate gate)
	{
		gate.EnableConflict();
		_gatesWithConflict.Add(gate);
	}

	private void RemoveGateFromConflicted(Gate gate)
	{
		gate.DisableConflict();
		_gatesWithConflict.Remove(gate);
	}

	private void TryOpenConflictedGates()
	{
		if (_gatesWithConflict.Count <= 0)
		{
			return;
		}
		_gatesWithConflictCache.AddRange(_gatesWithConflict);
		_gatesWithConflict.Clear();
		foreach (Gate item in _gatesWithConflictCache)
		{
			TryOpenGate(item);
		}
		_gatesWithConflictCache.Clear();
	}

	private void AddToOpenGateCrossings(GatePlacement gatePlacement)
	{
		_openGateCrossings[gatePlacement.Start] = gatePlacement.End;
		_openGateCrossings[gatePlacement.End] = gatePlacement.Start;
	}
}
