using System;
using System.Collections.Generic;
using Timberborn.BlockObjectModelSystem;
using Timberborn.BlockSystem;
using Timberborn.BlueprintSystem;
using Timberborn.BuildingsNavigation;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.GameDistricts;
using Timberborn.RootProviders;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.ZiplineSystem;

public class ZiplineConnectionService : ILoadableSingleton
{
	private readonly struct ConnectionAtCoordinates
	{
		public ZiplineTower First { get; }

		public ZiplineTower Second { get; }

		public Vector3Int Coordinates { get; }

		public BlockObject BlockObject { get; }

		public ConnectionAtCoordinates(ZiplineTower first, ZiplineTower second, Vector3Int coordinates, BlockObject blockObject)
		{
			First = first;
			Second = second;
			Coordinates = coordinates;
			BlockObject = blockObject;
		}
	}

	private readonly IBlockService _blockService;

	private readonly ZiplineConnectionBlockFactory _ziplineConnectionBlockFactory;

	private readonly RootObjectProvider _rootObjectProvider;

	private readonly ZiplineCableRenderer _ziplineCableRenderer;

	private readonly ZiplineCableNavMesh _ziplineCableNavMesh;

	private readonly BresenhamLineDrawer _bresenhamLineDrawer;

	private readonly BlockValidator _blockValidator;

	private readonly ISpecService _specService;

	private readonly EventBus _eventBus;

	private readonly List<ConnectionAtCoordinates> _connections = new List<ConnectionAtCoordinates>();

	private readonly HashSet<Vector3Int> _coordinatesCache = new HashSet<Vector3Int>();

	private Transform _root;

	private int _maxCableInclination;

	public ZiplineConnectionService(IBlockService blockService, ZiplineConnectionBlockFactory ziplineConnectionBlockFactory, RootObjectProvider rootObjectProvider, ZiplineCableRenderer ziplineCableRenderer, ZiplineCableNavMesh ziplineCableNavMesh, BresenhamLineDrawer bresenhamLineDrawer, BlockValidator blockValidator, ISpecService specService, EventBus eventBus)
	{
		_blockService = blockService;
		_ziplineConnectionBlockFactory = ziplineConnectionBlockFactory;
		_rootObjectProvider = rootObjectProvider;
		_ziplineCableRenderer = ziplineCableRenderer;
		_ziplineCableNavMesh = ziplineCableNavMesh;
		_bresenhamLineDrawer = bresenhamLineDrawer;
		_blockValidator = blockValidator;
		_specService = specService;
		_eventBus = eventBus;
	}

	public void Load()
	{
		_root = _rootObjectProvider.CreateRootObject("ZiplineConnectionService").transform;
		ZiplineConnectionServiceSpec singleSpec = _specService.GetSingleSpec<ZiplineConnectionServiceSpec>();
		_maxCableInclination = singleSpec.MaxCableInclination;
	}

	public bool CanBeConnected(ZiplineTower ziplineTower, ZiplineTower otherZiplineTower)
	{
		if ((bool)otherZiplineTower && otherZiplineTower != ziplineTower && otherZiplineTower.HasFreeSlots && !ziplineTower.IsConnectedTo(otherZiplineTower) && DistrictCentersAreCompatible(ziplineTower, otherZiplineTower))
		{
			return ConnectionIsValid(ziplineTower, otherZiplineTower);
		}
		return false;
	}

	public void GetBlockingObjects(ZiplineTower ziplineTower, ZiplineTower otherZiplineTower, IList<BlockObject> blockingObjects)
	{
		AddCoordinates(ziplineTower, otherZiplineTower);
		GetBlockingObjects(blockingObjects);
		_coordinatesCache.Clear();
	}

	public IEnumerable<Vector3Int> GetConnectionCoordinates()
	{
		foreach (ConnectionAtCoordinates connection in _connections)
		{
			yield return connection.Coordinates;
		}
	}

	public void Connect(ZiplineTower ziplineTower, ZiplineTower otherZiplineTower)
	{
		ziplineTower.AddConnection(otherZiplineTower);
		otherZiplineTower.AddConnection(ziplineTower);
		AddCoordinates(ziplineTower, otherZiplineTower);
		foreach (Vector3Int item in _coordinatesCache)
		{
			CableBlock bottomObjectComponentAt = _blockService.GetBottomObjectComponentAt<CableBlock>(item);
			BlockObject blockObject = (bottomObjectComponentAt ? bottomObjectComponentAt.GetComponent<BlockObject>() : _ziplineConnectionBlockFactory.CreateConnection(_root, item));
			_connections.Add(new ConnectionAtCoordinates(ziplineTower, otherZiplineTower, item, blockObject));
		}
		if (ziplineTower.IsActive && otherZiplineTower.IsActive)
		{
			_ziplineCableRenderer.AddActiveConnection(ziplineTower, otherZiplineTower);
			_ziplineCableNavMesh.AddActiveConnection(ziplineTower, otherZiplineTower);
			NotifyConnectionActivated(ziplineTower);
		}
		else
		{
			_ziplineCableRenderer.AddInactiveConnection(ziplineTower, otherZiplineTower);
			_ziplineCableNavMesh.AddInactiveConnection(ziplineTower, otherZiplineTower);
		}
		_coordinatesCache.Clear();
	}

	public void ActivateConnection(ZiplineTower ziplineTower, ZiplineTower otherZiplineTower)
	{
		_ziplineCableRenderer.ActivateConnection(ziplineTower, otherZiplineTower);
		_ziplineCableNavMesh.ActivateConnection(ziplineTower, otherZiplineTower);
		NotifyConnectionActivated(ziplineTower);
	}

	public void Disconnect(ZiplineTower ziplineTower, ZiplineTower otherZiplineTower)
	{
		ziplineTower.RemoveConnection(otherZiplineTower);
		otherZiplineTower.RemoveConnection(ziplineTower);
		for (int num = _connections.Count - 1; num >= 0; num--)
		{
			ConnectionAtCoordinates connectionAtCoordinates = _connections[num];
			if ((connectionAtCoordinates.First == ziplineTower && connectionAtCoordinates.Second == otherZiplineTower) || (connectionAtCoordinates.First == otherZiplineTower && connectionAtCoordinates.Second == ziplineTower))
			{
				_connections.RemoveAt(num);
				if (!IsConnectionAtCoordinates(connectionAtCoordinates.Coordinates))
				{
					connectionAtCoordinates.BlockObject.DeleteEntity();
					UnityEngine.Object.Destroy(connectionAtCoordinates.BlockObject.GameObject);
				}
			}
		}
		_ziplineCableRenderer.RemoveConnection(ziplineTower, otherZiplineTower);
		_ziplineCableNavMesh.RemoveConnection(ziplineTower, otherZiplineTower);
	}

	public bool InclinationIsValid(ZiplineTower ziplineTower, ZiplineTower otherZiplineTower, out float inclination, out float maxInclination)
	{
		(Vector3, Vector3) tuple = ZiplineCalculator.CalculateGridAnchors(ziplineTower.CableAnchorPoint, otherZiplineTower.CableAnchorPoint);
		Vector3 item = tuple.Item1;
		Vector3 normalized = (tuple.Item2 - item).normalized;
		inclination = Math.Abs(Vector3.Angle(new Vector3(0f, 0f, 1f), normalized) - 90f);
		maxInclination = _maxCableInclination;
		return inclination < maxInclination;
	}

	public bool DistanceIsValid(ZiplineTower ziplineTower, ZiplineTower otherZiplineTower, out float distance, out float maxDistance)
	{
		distance = Vector3.Distance(ziplineTower.CableAnchorPoint, otherZiplineTower.CableAnchorPoint);
		maxDistance = Math.Min(ziplineTower.MaxDistance, otherZiplineTower.MaxDistance);
		return distance <= maxDistance;
	}

	public bool DistrictCentersAreCompatible(ZiplineTower ziplineTower, ZiplineTower otherZiplineTower)
	{
		DistrictCenter districtCenter = GetDistrictCenter(ziplineTower);
		if ((bool)districtCenter)
		{
			DistrictCenter districtCenter2 = GetDistrictCenter(otherZiplineTower);
			if (districtCenter2 != null)
			{
				return districtCenter == districtCenter2;
			}
		}
		return true;
	}

	private bool ConnectionIsValid(ZiplineTower ziplineTower, ZiplineTower otherZiplineTower)
	{
		if (InclinationIsValid(ziplineTower, otherZiplineTower, out var inclination, out var maxInclination) && DistanceIsValid(ziplineTower, otherZiplineTower, out maxInclination, out inclination))
		{
			AddCoordinates(ziplineTower, otherZiplineTower);
			bool result = BlocksValid() && PreviewNotBlocking(ziplineTower, otherZiplineTower);
			_coordinatesCache.Clear();
			return result;
		}
		return false;
	}

	private void AddCoordinates(ZiplineTower ziplineTower, ZiplineTower otherZiplineTower)
	{
		Vector3Int start = ziplineTower.CableAnchorPoint.FloorToInt();
		Vector3Int end = otherZiplineTower.CableAnchorPoint.FloorToInt();
		_bresenhamLineDrawer.DrawLine(start, end, _coordinatesCache);
		RemoveNonBlockingCoordinates(ziplineTower);
		RemoveNonBlockingCoordinates(otherZiplineTower);
	}

	private void RemoveNonBlockingCoordinates(ZiplineTower ziplineTower)
	{
		_coordinatesCache.Remove(ziplineTower.CableAnchorPoint.FloorToInt());
		BlockObject component = ziplineTower.GetComponent<BlockObject>();
		foreach (Vector3Int unobstructedCoordinate in ziplineTower.UnobstructedCoordinates)
		{
			_coordinatesCache.Remove(component.TransformCoordinates(unobstructedCoordinate));
		}
	}

	private bool BlocksValid()
	{
		BlockObjectSpec ziplineConnectionBlock = _ziplineConnectionBlockFactory.ZiplineConnectionBlock;
		foreach (Vector3Int item in _coordinatesCache)
		{
			if (!_blockValidator.BlocksValid(ziplineConnectionBlock, new Placement(item)) && !_blockService.GetBottomObjectComponentAt<CableBlock>(item))
			{
				return false;
			}
		}
		return true;
	}

	private bool PreviewNotBlocking(ZiplineTower ziplineTower, ZiplineTower otherZiplineTower)
	{
		BlockObject component = ziplineTower.GetComponent<BlockObject>();
		BlockObject component2 = otherZiplineTower.GetComponent<BlockObject>();
		if (component.IsPreview && PreviewIsBlocking(component))
		{
			return false;
		}
		if (component2.IsPreview && PreviewIsBlocking(component2))
		{
			return false;
		}
		return true;
	}

	private bool PreviewIsBlocking(BlockObject blockObject)
	{
		BlockOccupations ziplineConnectionOccupation = _ziplineConnectionBlockFactory.ZiplineConnectionOccupation;
		foreach (Vector3Int item in blockObject.PositionedBlocks.GetOccupiedCoordinatesIntersecting(ziplineConnectionOccupation))
		{
			if (_coordinatesCache.Contains(item))
			{
				return true;
			}
		}
		return false;
	}

	private void GetBlockingObjects(IList<BlockObject> blockingObjects)
	{
		foreach (Vector3Int item in _coordinatesCache)
		{
			if (!_blockService.AnyObjectAt(item) || (bool)_blockService.GetBottomObjectComponentAt<CableBlock>(item))
			{
				continue;
			}
			foreach (BlockObject item2 in _blockService.GetObjectsAt(item))
			{
				if (item2.HasComponent<IBlockObjectModel>())
				{
					blockingObjects.Add(item2);
				}
			}
		}
	}

	private bool IsConnectionAtCoordinates(Vector3Int coordinates)
	{
		for (int i = 0; i < _connections.Count; i++)
		{
			if (coordinates == _connections[i].Coordinates)
			{
				return true;
			}
		}
		return false;
	}

	private void NotifyConnectionActivated(ZiplineTower ziplineTower)
	{
		_eventBus.Post(new ZiplineConnectionActivatedEvent(ziplineTower));
	}

	private static DistrictCenter GetDistrictCenter(ZiplineTower ziplineTower)
	{
		return ziplineTower.GetComponent<PathDistrictRetriever>().GetAnyDistrictCenter();
	}
}
