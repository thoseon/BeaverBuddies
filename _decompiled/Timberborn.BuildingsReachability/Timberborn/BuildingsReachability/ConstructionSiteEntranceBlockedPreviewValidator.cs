using System.Collections.Generic;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockObjectAccesses;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.Localization;
using UnityEngine;

namespace Timberborn.BuildingsReachability;

internal class ConstructionSiteEntranceBlockedPreviewValidator : BaseComponent, IAwakableComponent, IPreviewValidator
{
	private static readonly string EntranceBlockedLocKey = "Buildings.EntranceBlocked";

	private readonly IBlockService _blockService;

	private readonly NeighborCalculator _neighborCalculator;

	private readonly ILoc _loc;

	private BlockableEntranceBuilding _blockableEntranceBuilding;

	private BlockObject _blockObject;

	private readonly HashSet<BlockableEntranceBuilding> _candidateBuildings = new HashSet<BlockableEntranceBuilding>();

	private readonly HashSet<BaseComponent> _blockedBuildings = new HashSet<BaseComponent>();

	private IEnumerable<Vector3Int> PreviewCoordinates => from block in _blockObject.PositionedBlocks.GetOccupiedBlocks()
		where block.Occupation.HasBottomOrFloorOrFull()
		select block.Coordinates;

	public ConstructionSiteEntranceBlockedPreviewValidator(IBlockService blockService, NeighborCalculator neighborCalculator, ILoc loc)
	{
		_blockService = blockService;
		_neighborCalculator = neighborCalculator;
		_loc = loc;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_blockableEntranceBuilding = GetComponent<BlockableEntranceBuilding>();
	}

	public bool IsValid(out string warningMessage)
	{
		warningMessage = _loc.T(EntranceBlockedLocKey);
		return !_blockableEntranceBuilding.IsEntranceBlocked();
	}

	public ReadOnlyHashSet<BaseComponent> InvalidatedObjects(out string warningMessage)
	{
		warningMessage = _loc.T(EntranceBlockedLocKey);
		_candidateBuildings.Clear();
		_blockedBuildings.Clear();
		foreach (Vector3Int item in _neighborCalculator.GetNonInternalNeighborsWithoutDiagonal(PreviewCoordinates))
		{
			BlockableEntranceBuilding bottomObjectComponentAt = _blockService.GetBottomObjectComponentAt<BlockableEntranceBuilding>(item);
			if ((bool)bottomObjectComponentAt)
			{
				_candidateBuildings.Add(bottomObjectComponentAt);
			}
		}
		foreach (BlockableEntranceBuilding candidateBuilding in _candidateBuildings)
		{
			if (candidateBuilding.IsEntranceBlockedByCoordinates(PreviewCoordinates))
			{
				_blockedBuildings.Add(candidateBuilding);
			}
		}
		return _blockedBuildings.AsReadOnlyHashSet();
	}
}
