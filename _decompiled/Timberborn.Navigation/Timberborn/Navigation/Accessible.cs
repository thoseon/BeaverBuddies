using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.Navigation;

public class Accessible : BaseComponent, IAwakableComponent, INamedComponent
{
	private readonly INavigationService _navigationService;

	private IPathToAccessibleModifier _pathModifier;

	private readonly List<IAccessibleValidator> _accessibleValidators = new List<IAccessibleValidator>();

	private IBlockedAccessible _blockedAccessible;

	private readonly List<Vector3> _accesses = new List<Vector3>();

	private readonly List<PathCorner> _tempPathCorners = new List<PathCorner>();

	private Vector3? _finalAccess;

	public string ComponentName { get; private set; }

	public ReadOnlyList<Vector3> Accesses => _accesses.AsReadOnlyList();

	public bool ValidAccessible
	{
		get
		{
			if (base.Enabled)
			{
				return IsValid();
			}
			return false;
		}
	}

	public Vector3? UnblockedSingleAccess
	{
		get
		{
			if (!base.Enabled || IsBlocked)
			{
				return null;
			}
			return _accesses.Single();
		}
	}

	public Vector3? UnblockedSingleAccessInstant
	{
		get
		{
			if (!base.Enabled || IsBlockedInstant)
			{
				return null;
			}
			return _accesses.Single();
		}
	}

	public bool HasSingleAccess => _accesses.Count == 1;

	private bool IsBlocked => _blockedAccessible?.IsBlocked() ?? false;

	private bool IsBlockedInstant => _blockedAccessible?.IsBlockedInstant() ?? false;

	public Accessible(INavigationService navigationService)
	{
		_navigationService = navigationService;
	}

	public void Awake()
	{
		DisableComponent();
		_pathModifier = GetComponent<IPathToAccessibleModifier>();
	}

	public void Initialize(string componentName)
	{
		ComponentName = componentName;
		_blockedAccessible = GetComponent<IBlockedAccessible>();
	}

	public void SetAccesses(IEnumerable<Vector3> accesses, Vector3? finalAccess = null)
	{
		GetComponents(_accessibleValidators);
		_accesses.Clear();
		_accesses.AddRange(accesses);
		_finalAccess = finalAccess;
		EnableComponent();
	}

	public void ClearAccesses()
	{
		_accesses.Clear();
		DisableComponent();
	}

	public bool IsReachableByRoad(Accessible end)
	{
		float distance;
		return FindRoadPath(end, out distance);
	}

	public bool FindRoadPath(Vector3 end, out float distance)
	{
		Vector3? unblockedSingleAccess = UnblockedSingleAccess;
		if (unblockedSingleAccess.HasValue)
		{
			Vector3 valueOrDefault = unblockedSingleAccess.GetValueOrDefault();
			return _navigationService.FindRoadPath(valueOrDefault, end, out distance);
		}
		distance = 0f;
		return false;
	}

	public bool FindRoadPath(Accessible end, out float distance)
	{
		Vector3? unblockedSingleAccess = UnblockedSingleAccess;
		if (unblockedSingleAccess.HasValue)
		{
			Vector3 valueOrDefault = unblockedSingleAccess.GetValueOrDefault();
			float num = float.PositiveInfinity;
			bool result = false;
			if (!end.IsBlocked)
			{
				List<Vector3> accesses = end._accesses;
				for (int i = 0; i < accesses.Count; i++)
				{
					if (_navigationService.FindRoadPath(valueOrDefault, accesses[i], out var distance2))
					{
						num = Math.Min(num, distance2);
						result = true;
					}
				}
			}
			distance = num;
			return result;
		}
		distance = 0f;
		return false;
	}

	public bool FindInstantRoadPath(Vector3 end, out float distance)
	{
		Vector3? unblockedSingleAccess = UnblockedSingleAccess;
		if (unblockedSingleAccess.HasValue)
		{
			Vector3 valueOrDefault = unblockedSingleAccess.GetValueOrDefault();
			return _navigationService.FindInstantRoadPath(valueOrDefault, end, out distance);
		}
		distance = 0f;
		return false;
	}

	public bool FindInstantRoadPath(Accessible end, out float distance)
	{
		Vector3? unblockedSingleAccess = UnblockedSingleAccess;
		if (unblockedSingleAccess.HasValue)
		{
			Vector3 valueOrDefault = unblockedSingleAccess.GetValueOrDefault();
			float num = float.PositiveInfinity;
			bool result = false;
			if (!end.IsBlocked)
			{
				List<Vector3> accesses = end._accesses;
				for (int i = 0; i < accesses.Count; i++)
				{
					if (_navigationService.FindInstantRoadPath(valueOrDefault, accesses[i], out var distance2))
					{
						num = Math.Min(num, distance2);
						result = true;
					}
				}
			}
			distance = num;
			return result;
		}
		distance = 0f;
		return false;
	}

	public bool IsReachableByTerrain(Vector3 end)
	{
		float distance;
		return FindTerrainPath(end, out distance);
	}

	public bool IsReachableByRoadToTerrain(Accessible end)
	{
		Vector3 endOfRoad;
		float totalDistance;
		return FindRoadToTerrainPath(end, out endOfRoad, out totalDistance);
	}

	public bool FindTerrainPath(Vector3 end, out float distance)
	{
		Vector3? unblockedSingleAccess = UnblockedSingleAccess;
		if (unblockedSingleAccess.HasValue)
		{
			Vector3 valueOrDefault = unblockedSingleAccess.GetValueOrDefault();
			return _navigationService.FindTerrainPath(valueOrDefault, end, out distance);
		}
		distance = 0f;
		return false;
	}

	public bool FindTerrainPath(Accessible end, out float distance)
	{
		Vector3? unblockedSingleAccess = UnblockedSingleAccess;
		if (unblockedSingleAccess.HasValue)
		{
			Vector3 valueOrDefault = unblockedSingleAccess.GetValueOrDefault();
			float num = float.PositiveInfinity;
			bool result = false;
			if (!end.IsBlocked)
			{
				List<Vector3> accesses = end._accesses;
				for (int i = 0; i < accesses.Count; i++)
				{
					if (_navigationService.FindTerrainPath(valueOrDefault, accesses[i], out var distance2))
					{
						num = Math.Min(num, distance2);
						result = true;
					}
				}
			}
			distance = num;
			return result;
		}
		distance = 0f;
		return false;
	}

	public bool FindRoadToTerrainPath(Accessible end, out Vector3 endOfRoad, out float totalDistance)
	{
		Vector3? unblockedSingleAccess = UnblockedSingleAccess;
		if (unblockedSingleAccess.HasValue)
		{
			Vector3 valueOrDefault = unblockedSingleAccess.GetValueOrDefault();
			float num = float.PositiveInfinity;
			endOfRoad = default(Vector3);
			totalDistance = 0f;
			bool result = false;
			if (!end.IsBlocked)
			{
				List<Vector3> accesses = end._accesses;
				for (int i = 0; i < accesses.Count; i++)
				{
					if (_navigationService.FindRoadToTerrainPath(valueOrDefault, accesses[i], out var endOfRoad2, out var distanceFromClosestRoad, out var totalDistance2) && num > distanceFromClosestRoad)
					{
						num = distanceFromClosestRoad;
						endOfRoad = endOfRoad2;
						totalDistance = totalDistance2;
						result = true;
					}
				}
			}
			return result;
		}
		endOfRoad = default(Vector3);
		totalDistance = 0f;
		return false;
	}

	public bool FindRoadToTerrainPath(Vector3 end, out float totalDistance)
	{
		Vector3? unblockedSingleAccess = UnblockedSingleAccess;
		if (unblockedSingleAccess.HasValue)
		{
			Vector3 valueOrDefault = unblockedSingleAccess.GetValueOrDefault();
			Vector3 endOfRoad;
			float distanceFromClosestRoad;
			return _navigationService.FindRoadToTerrainPath(valueOrDefault, end, out endOfRoad, out distanceFromClosestRoad, out totalDistance);
		}
		totalDistance = 0f;
		return false;
	}

	public bool FindPathUnlimitedRange(Vector3 start, List<PathCorner> pathCorners, out float distance)
	{
		if (!IsBlocked && _navigationService.FindPathUnlimitedRange(start, _accesses, pathCorners, out distance))
		{
			Vector3? finalAccess = _finalAccess;
			if (finalAccess.HasValue)
			{
				Vector3 valueOrDefault = finalAccess.GetValueOrDefault();
				if (!_navigationService.FindPathUnlimitedRange(pathCorners.Last().Position, valueOrDefault, _tempPathCorners, out var _))
				{
					pathCorners.Clear();
					distance = 0f;
					return false;
				}
				if (_tempPathCorners.Count >= pathCorners.Count)
				{
					return _navigationService.FindPathUnlimitedRange(start, valueOrDefault, pathCorners, out distance);
				}
				pathCorners.RemoveLast();
				pathCorners.AddRange(_tempPathCorners);
			}
			_pathModifier?.ModifyPath(start, pathCorners, ref distance);
			return true;
		}
		pathCorners.Clear();
		distance = 0f;
		return false;
	}

	public Vector3 FindExitPath(Vector3 start, List<PathCorner> pathCorners)
	{
		if (_finalAccess.HasValue)
		{
			Vector3? unblockedSingleAccess = UnblockedSingleAccess;
			if (unblockedSingleAccess.HasValue)
			{
				Vector3 valueOrDefault = unblockedSingleAccess.GetValueOrDefault();
				if (_navigationService.FindPathUnlimitedRange(start, valueOrDefault, pathCorners, out var _))
				{
					return valueOrDefault;
				}
			}
		}
		return start;
	}

	public bool IsReachableUnlimitedRange(Vector3 start)
	{
		if (!IsBlocked)
		{
			for (int i = 0; i < _accesses.Count; i++)
			{
				if (_navigationService.DestinationIsReachableUnlimitedRange(start, _accesses[i]))
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool IsValid()
	{
		foreach (IAccessibleValidator accessibleValidator in _accessibleValidators)
		{
			if (!accessibleValidator.ValidAccessible)
			{
				return false;
			}
		}
		return true;
	}
}
