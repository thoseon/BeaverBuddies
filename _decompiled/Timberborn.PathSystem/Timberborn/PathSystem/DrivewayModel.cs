using System;
using Timberborn.BlockSystem;
using Timberborn.Coordinates;
using Timberborn.TerrainSystem;
using UnityEngine;

namespace Timberborn.PathSystem;

internal class DrivewayModel
{
	private readonly IConnectionService _connectionService;

	private readonly ITerrainService _terrainService;

	private readonly DrivewayModelInstantiator _drivewayModelInstantiator;

	private readonly IBlockService _blockService;

	private readonly BlockObject _blockObject;

	private readonly DrivewayModelSpec _drivewayModelSpec;

	private GameObject _model;

	public Driveway Driveway => _drivewayModelSpec.Driveway;

	internal DrivewayModel(IConnectionService connectionService, ITerrainService terrainService, DrivewayModelInstantiator drivewayModelInstantiator, IBlockService blockService, BlockObject blockObject, DrivewayModelSpec drivewayModelSpec)
	{
		_connectionService = connectionService;
		_terrainService = terrainService;
		_drivewayModelInstantiator = drivewayModelInstantiator;
		_blockService = blockService;
		_blockObject = blockObject;
		_drivewayModelSpec = drivewayModelSpec;
	}

	internal void UpdateModel()
	{
		if ((object)_model == null)
		{
			_model = _drivewayModelInstantiator.InstantiateModel(this, _blockObject, GetLocalCoordinates(), GetLocalDirection());
		}
		Vector3Int positionedCoordinates = GetPositionedCoordinates();
		Direction2D positionedDirection = GetPositionedDirection();
		bool flag = _connectionService.CanConnectInDirection(positionedCoordinates, positionedDirection);
		Direction2D secondPositionedDirection = GetSecondPositionedDirection();
		switch (_drivewayModelSpec.DrivewayMode)
		{
		case DrivewayMode.StrictlyUnidirectional:
			flag &= !_connectionService.CanConnectInDirection(positionedCoordinates, secondPositionedDirection);
			break;
		case DrivewayMode.Bidirectional:
			flag &= _connectionService.CanConnectInDirection(positionedCoordinates, secondPositionedDirection);
			break;
		default:
			throw new NotSupportedException($"Unexpected value: {_drivewayModelSpec.DrivewayMode}");
		case DrivewayMode.Unidirectional:
			break;
		}
		bool flag2 = _terrainService.OnGround(positionedCoordinates);
		bool active = flag && (flag2 || IsOnBlockObjectWithGroundPath(positionedCoordinates));
		_model.SetActive(active);
	}

	internal void ValidateDriveway()
	{
		if (!_blockObject.Entrance.HasEntrance && !_drivewayModelSpec.HasCustomCoordinates)
		{
			throw new Exception("BlockObject " + _blockObject.Name + " with DrivewayModel must have Entrance or custom coordinates.");
		}
		if (Driveway == Driveway.None)
		{
			throw new Exception("DrivewayModel " + _blockObject.Name + " must have Driveway set to a value different than None.");
		}
	}

	private Vector3Int GetLocalCoordinates()
	{
		if (!_drivewayModelSpec.HasCustomCoordinates)
		{
			return _blockObject.Entrance.Coordinates;
		}
		return _drivewayModelSpec.CustomCoordinates;
	}

	private Direction2D GetLocalDirection()
	{
		if (!_drivewayModelSpec.HasCustomCoordinates)
		{
			return Direction2D.Down;
		}
		return _drivewayModelSpec.CustomDirection;
	}

	private Vector3Int GetPositionedCoordinates()
	{
		if (!_drivewayModelSpec.HasCustomCoordinates)
		{
			return _blockObject.PositionedEntrance.DoorstepCoordinates;
		}
		return _blockObject.TransformCoordinates(_drivewayModelSpec.CustomCoordinates - _drivewayModelSpec.CustomDirection.ToOffset());
	}

	private Direction2D GetPositionedDirection()
	{
		if (!(_drivewayModelSpec != null))
		{
			return _blockObject.PositionedEntrance.Direction2D;
		}
		return _blockObject.TransformDirection(_drivewayModelSpec.CustomDirection);
	}

	private Direction2D GetSecondPositionedDirection()
	{
		return GetPositionedDirection().Next().Next();
	}

	private bool IsOnBlockObjectWithGroundPath(Vector3Int coordinates)
	{
		PathModelTypeEnforcerSpec topObjectComponentAt = _blockService.GetTopObjectComponentAt<PathModelTypeEnforcerSpec>(coordinates - new Vector3Int(0, 0, 1));
		if ((object)topObjectComponentAt != null)
		{
			return topObjectComponentAt.PathModelType == PathModelType.Ground;
		}
		return false;
	}
}
