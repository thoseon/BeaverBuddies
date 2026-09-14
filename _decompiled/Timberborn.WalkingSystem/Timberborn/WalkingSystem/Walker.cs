using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.CharacterMovementSystem;
using Timberborn.Common;
using Timberborn.EnterableSystem;
using Timberborn.GameDistricts;
using Timberborn.Navigation;
using Timberborn.Persistence;
using Timberborn.TickSystem;
using Timberborn.TimeSystem;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.WalkingSystem;

public class Walker : TickableComponent, IAwakableComponent, IPersistentEntity
{
	private static readonly ComponentKey WalkerKey = new ComponentKey("Walker");

	private static readonly PropertyKey<IDestination> CurrentDestinationKey = new PropertyKey<IDestination>("CurrentDestination");

	private readonly PathFollowerFactory _pathFollowerFactory;

	private readonly DestinationValueSerializer _destinationValueSerializer;

	private readonly INavigationService _navigationService;

	private readonly IDayNightCycle _dayNightCycle;

	private readonly IDistrictService _districtService;

	private Enterer _enterer;

	private Citizen _citizen;

	private WalkerSpeedManager _walkerSpeedManager;

	private WalkerPathStart _walkerPathStart;

	private readonly List<PathCorner> _pathCorners = new List<PathCorner>(100);

	private readonly List<PathCorner> _tempPathCorners = new List<PathCorner>(100);

	private IDestination _currentDestination;

	private bool _stopNextTick;

	public bool CurrentDestinationReachable { get; private set; }

	public BoundingBox CurrentPathBounds { get; private set; }

	public PathFollower PathFollower { get; private set; }

	public ReadOnlyList<PathCorner> PathCorners => _pathCorners.AsReadOnlyList();

	public event EventHandler<StartedNewPathEventArgs> StartedNewPath;

	public Walker(PathFollowerFactory pathFollowerFactory, DestinationValueSerializer destinationValueSerializer, INavigationService navigationService, IDayNightCycle dayNightCycle, IDistrictService districtService)
	{
		_pathFollowerFactory = pathFollowerFactory;
		_destinationValueSerializer = destinationValueSerializer;
		_navigationService = navigationService;
		_dayNightCycle = dayNightCycle;
		_districtService = districtService;
	}

	public void Awake()
	{
		_enterer = GetComponent<Enterer>();
		_citizen = GetComponent<Citizen>();
		_walkerSpeedManager = GetComponent<WalkerSpeedManager>();
		_walkerPathStart = GetComponent<WalkerPathStart>();
		PathFollower = _pathFollowerFactory.Create(this);
	}

	public override void Tick()
	{
		if (!Stopped() && (IsOutsideAndReachedDestination() || _stopNextTick))
		{
			StopMoving();
		}
	}

	public ExecutorStatus GoTo(IDestination destination)
	{
		return FindPath(destination);
	}

	public void StopNextTick()
	{
		_stopNextTick = true;
	}

	public bool Stopped()
	{
		return _currentDestination == null;
	}

	public void RefreshPath()
	{
		if (_currentDestination != null)
		{
			FindPath(_currentDestination);
		}
	}

	public void Save(IEntitySaver entitySaver)
	{
		if (_currentDestination != null)
		{
			entitySaver.GetComponent(WalkerKey).Set(CurrentDestinationKey, _currentDestination, _destinationValueSerializer);
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(WalkerKey, out var objectLoader))
		{
			_currentDestination = objectLoader.Get(CurrentDestinationKey, _destinationValueSerializer);
		}
	}

	public float CalculateTravelTimeInHours(Vector3 start, Vector3 destination)
	{
		if (!_navigationService.FindPathUnlimitedRange(start, destination, null, out var distance))
		{
			distance = _navigationService.HeuristicDistance(start, destination);
		}
		float seconds = distance / _walkerSpeedManager.GetWalkerBaseSpeed();
		return _dayNightCycle.SecondsToHours(seconds);
	}

	private void StopMoving()
	{
		_currentDestination = null;
		_stopNextTick = false;
		PathFollower.StopMoving();
	}

	private ExecutorStatus FindPath(IDestination destination)
	{
		if (destination == null)
		{
			throw new NullReferenceException("Walker destination is null.");
		}
		_stopNextTick = false;
		_pathCorners.Clear();
		_tempPathCorners.Clear();
		_walkerPathStart.GetPathStart(destination, _pathCorners, out var start);
		CurrentDestinationReachable = destination.FindPath(start, _tempPathCorners, out var distance) && !PathIsTooFarFromDistrict(start, distance);
		if (CurrentDestinationReachable)
		{
			_pathCorners.AddRange(_tempPathCorners);
			PathFollower.StartMovingAlongPath(_pathCorners);
			StartedNewPath?.Invoke(this, new StartedNewPathEventArgs(distance));
			_currentDestination = destination;
			RecalculatePathBounds();
			if (IsOutsideAndReachedDestination())
			{
				StopMoving();
				return ExecutorStatus.Success;
			}
			return ExecutorStatus.Running;
		}
		_citizen.UnassignDistrictIfCutOff();
		StopMoving();
		return ExecutorStatus.Failure;
	}

	private void RecalculatePathBounds()
	{
		BoundingBox.Builder builder = default(BoundingBox.Builder);
		for (int i = 0; i < _pathCorners.Count; i++)
		{
			builder.Expand(NavigationCoordinateSystem.WorldToGridInt(_pathCorners[i].Position));
		}
		CurrentPathBounds = builder.Build();
	}

	private bool IsOutsideAndReachedDestination()
	{
		if (!_enterer.IsInside)
		{
			return PathFollower.ReachedLastPathCorner();
		}
		return false;
	}

	private bool PathIsTooFarFromDistrict(Vector3 start, float pathLength)
	{
		if ((bool)_citizen.AssignedDistrict && pathLength > (float)NavigationLimits.MaxEdgeCost)
		{
			return _districtService.IsOnInstantDistrictRoadSpill(start);
		}
		return false;
	}
}
