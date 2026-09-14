using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.CharacterMovementSystem;
using Timberborn.Coordinates;
using Timberborn.EntitySystem;
using Timberborn.GameDistricts;
using Timberborn.Navigation;
using Timberborn.Persistence;
using Timberborn.WalkingSystem;
using Timberborn.WorldPersistence;
using Timberborn.ZiplineSystem;
using UnityEngine;

namespace Timberborn.ZiplineMovementSystem;

internal class ZiplinePathTracker : BaseComponent, IAwakableComponent, IInitializableEntity, IPostLoadableEntity, IPersistentEntity, INavMeshProximityValidator, IPathStartProvider, ICitizenPositionOverrider
{
	private static readonly ComponentKey ZiplinePathTrackerKey = new ComponentKey("ZiplinePathTracker");

	private static readonly PropertyKey<Vector3> FromPointKey = new PropertyKey<Vector3>("FromPoint");

	private static readonly PropertyKey<Vector3> ToPointKey = new PropertyKey<Vector3>("ToPoint");

	private static readonly PropertyKey<Vector3> NextTurnPointKey = new PropertyKey<Vector3>("NextTurnPoint");

	private static readonly PropertyKey<float> LastMovementSpeedKey = new PropertyKey<float>("LastMovementSpeed");

	private readonly IBlockService _blockService;

	private readonly ZiplineGroupService _ziplineGroupService;

	private Walker _walker;

	private Vector3? _fromPoint;

	private Vector3? _toPoint;

	private Vector3? _nextTurnPoint;

	private float _lastMovementSpeed;

	public ZiplinePathTracker(IBlockService blockService, ZiplineGroupService ziplineGroupService)
	{
		_blockService = blockService;
		_ziplineGroupService = ziplineGroupService;
	}

	public void Awake()
	{
		_walker = GetComponent<Walker>();
	}

	public void InitializeEntity()
	{
		_walker.PathFollower.MovedAlongPath += OnMovedAlongPath;
	}

	public void Save(IEntitySaver entitySaver)
	{
		if (_fromPoint.HasValue && _toPoint.HasValue)
		{
			IObjectSaver component = entitySaver.GetComponent(ZiplinePathTrackerKey);
			component.Set(FromPointKey, _fromPoint.Value);
			component.Set(ToPointKey, _toPoint.Value);
			component.Set(LastMovementSpeedKey, _lastMovementSpeed);
			if (_nextTurnPoint.HasValue)
			{
				component.Set(NextTurnPointKey, _nextTurnPoint.Value);
			}
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(ZiplinePathTrackerKey, out var objectLoader))
		{
			_fromPoint = objectLoader.Get(FromPointKey);
			_toPoint = objectLoader.Get(ToPointKey);
			_lastMovementSpeed = objectLoader.Get(LastMovementSpeedKey);
			if (objectLoader.Has(NextTurnPointKey))
			{
				_nextTurnPoint = objectLoader.Get(NextTurnPointKey);
			}
		}
	}

	public void PostLoadEntity()
	{
		ValidateCurrentEdge();
	}

	public bool IsOnNavMesh()
	{
		ValidateCurrentEdge();
		Vector3 destination;
		return IsCurrentlyOnZiplineEdge(out destination);
	}

	public bool TryGetPathStart(IDestination destination, List<PathCorner> pathCorners, out Vector3 start)
	{
		if (IsCurrentlyOnZiplineEdge(out var destination2))
		{
			start = destination2;
			float lastMovementSpeed = _lastMovementSpeed;
			pathCorners.Add(new PathCorner(base.Transform.position, lastMovementSpeed, _ziplineGroupService.PathStartGroupId));
			pathCorners.Add(new PathCorner(destination2, lastMovementSpeed, _ziplineGroupService.RegularGroupId));
			if (_nextTurnPoint.HasValue)
			{
				pathCorners.Add(new PathCorner(_nextTurnPoint.Value, lastMovementSpeed, _ziplineGroupService.TurnGroupId));
			}
			return true;
		}
		start = default(Vector3);
		return false;
	}

	public bool TryGetOverridenPosition(out Vector3 position)
	{
		return IsCurrentlyOnZiplineEdge(out position);
	}

	private void ValidateCurrentEdge()
	{
		if (!ZiplineEdgeHasValidConnection())
		{
			ClearCurrentEdge();
		}
	}

	private void OnMovedAlongPath(object sender, MovementEventArgs movementEventArgs)
	{
		if (IsMovingOnZiplineEdge(movementEventArgs))
		{
			PathCorner fromCorner = movementEventArgs.From;
			PathCorner to = movementEventArgs.To;
			if (EnteredNewZiplineEdge(fromCorner, to))
			{
				_fromPoint = fromCorner.Position;
				_toPoint = to.Position;
				_lastMovementSpeed = fromCorner.Speed;
				PeekNextTurningCorner(movementEventArgs);
			}
		}
		else
		{
			ClearCurrentEdge();
		}
	}

	private bool IsMovingOnZiplineEdge(MovementEventArgs movementEventArgs)
	{
		if (_ziplineGroupService.IsAnyZiplineGroup(movementEventArgs.From.GroupId))
		{
			return _ziplineGroupService.IsAnyZiplineGroup(movementEventArgs.To.GroupId);
		}
		return false;
	}

	private bool EnteredNewZiplineEdge(PathCorner fromCorner, PathCorner toCorner)
	{
		if (_ziplineGroupService.IsRegularEdge(fromCorner.GroupId, toCorner.GroupId) || _ziplineGroupService.IsTurnEdge(toCorner.GroupId, fromCorner.GroupId))
		{
			if (!_fromPoint.HasValue || _fromPoint != fromCorner.Position)
			{
				if (_toPoint.HasValue)
				{
					return _toPoint != toCorner.Position;
				}
				return true;
			}
			return false;
		}
		return false;
	}

	private bool ZiplineEdgeHasValidConnection()
	{
		if (_fromPoint.HasValue && _toPoint.HasValue)
		{
			Vector3Int vector3Int = CoordinateSystem.WorldToGridInt(_fromPoint.Value);
			Vector3Int vector3Int2 = CoordinateSystem.WorldToGridInt(_toPoint.Value);
			ZiplineTower bottomObjectComponentAt = _blockService.GetBottomObjectComponentAt<ZiplineTower>(vector3Int);
			ZiplineTower bottomObjectComponentAt2 = _blockService.GetBottomObjectComponentAt<ZiplineTower>(vector3Int2);
			if ((bool)bottomObjectComponentAt && (bool)bottomObjectComponentAt2 && vector3Int == bottomObjectComponentAt.CableAnchorPointInt && vector3Int2 == bottomObjectComponentAt2.CableAnchorPointInt)
			{
				if (bottomObjectComponentAt != bottomObjectComponentAt2)
				{
					return bottomObjectComponentAt.IsConnectedTo(bottomObjectComponentAt2);
				}
				return true;
			}
			return false;
		}
		return false;
	}

	private bool IsCurrentlyOnZiplineEdge(out Vector3 destination)
	{
		destination = _toPoint.GetValueOrDefault();
		if (_fromPoint.HasValue)
		{
			return _toPoint.HasValue;
		}
		return false;
	}

	private void PeekNextTurningCorner(MovementEventArgs movementEventArgs)
	{
		if (movementEventArgs.Next.HasValue && _ziplineGroupService.IsTurnEdge(movementEventArgs.To.GroupId, movementEventArgs.Next.Value.GroupId))
		{
			_nextTurnPoint = movementEventArgs.Next.Value.Position;
		}
		else
		{
			_nextTurnPoint = null;
		}
	}

	private void ClearCurrentEdge()
	{
		_fromPoint = null;
		_toPoint = null;
		_nextTurnPoint = null;
		_lastMovementSpeed = 0f;
	}
}
