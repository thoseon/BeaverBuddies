using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Demolishing;
using Timberborn.EntitySystem;
using Timberborn.Localization;
using Timberborn.StatusSystem;
using Timberborn.TerrainPhysics;
using Timberborn.TickSystem;
using UnityEngine;

namespace Timberborn.DemolishingUI;

internal class DemolitionBlockedStatus : TickableComponent, IAwakableComponent, IInitializableEntity
{
	private static readonly string BlockedKey = "Demolish.Blocked";

	private readonly IBlockService _blockService;

	private readonly ILoc _loc;

	private readonly ITerrainPhysicsService _terrainPhysicsService;

	private Demolishable _demolishable;

	private BlockObject _blockObject;

	private readonly List<Vector3Int> _stackableCoordinates = new List<Vector3Int>();

	private StatusToggle _demolitionBlockedStatus;

	public DemolitionBlockedStatus(IBlockService blockService, ILoc loc, ITerrainPhysicsService terrainPhysicsService)
	{
		_blockService = blockService;
		_loc = loc;
		_terrainPhysicsService = terrainPhysicsService;
	}

	public void Awake()
	{
		_demolishable = GetComponent<Demolishable>();
		_blockObject = GetComponent<BlockObject>();
		_demolitionBlockedStatus = StatusToggle.CreateNormalStatus("DemolitionBlocked", _loc.T(BlockedKey));
		_demolishable.Marked += OnMarked;
		_demolishable.Unmarked += delegate
		{
			DisableComponent();
		};
		DisableComponent();
	}

	public void InitializeEntity()
	{
		if (!_blockObject.IsPreview)
		{
			Vector3Int offset = new Vector3Int(0, 0, 1);
			IEnumerable<Vector3Int> collection = from block in _blockObject.PositionedBlocks.GetOccupiedBlocks()
				where block.Stackable.IsStackable()
				select block.Coordinates + offset;
			_stackableCoordinates.AddRange(collection);
			if (_demolishable.IsMarked)
			{
				Enable();
			}
		}
		GetComponent<StatusSubject>().RegisterStatus(_demolitionBlockedStatus);
	}

	public override void Tick()
	{
		UpdateStatus();
	}

	private void OnMarked(object sender, EventArgs e)
	{
		Enable();
	}

	private void UpdateStatus()
	{
		if (IsBlocked())
		{
			_demolitionBlockedStatus.Activate();
		}
		else
		{
			_demolitionBlockedStatus.Deactivate();
		}
	}

	private bool IsBlocked()
	{
		foreach (Vector3Int stackableCoordinate in _stackableCoordinates)
		{
			foreach (BlockObject item in _blockService.GetStackedObjectsAt(stackableCoordinate))
			{
				Demolishable component = item.GetComponent<Demolishable>();
				if (component == null || !component.IsMarked)
				{
					return true;
				}
			}
		}
		return !_terrainPhysicsService.CanBeDestroyed(_blockObject);
	}

	private void Enable()
	{
		if (_stackableCoordinates.Count > 0)
		{
			EnableComponent();
		}
	}
}
