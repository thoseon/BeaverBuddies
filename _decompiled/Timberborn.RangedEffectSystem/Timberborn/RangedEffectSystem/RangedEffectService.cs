using System.Collections.Generic;
using System.Linq;
using Timberborn.BlockSystem;
using Timberborn.Buildings;
using Timberborn.Common;
using Timberborn.EnterableSystem;
using Timberborn.SingletonSystem;
using Timberborn.TerrainSystem;
using UnityEngine;

namespace Timberborn.RangedEffectSystem;

internal class RangedEffectService : ILoadableSingleton
{
	private static readonly List<RangedEffect> EmptyEffects = new List<RangedEffect>();

	private readonly EventBus _eventBus;

	private readonly ITerrainService _terrainService;

	private readonly IBlockService _blockService;

	private RangedEffects[,] _rangedEffects;

	public RangedEffectService(ITerrainService terrainService, EventBus eventBus, IBlockService blockService)
	{
		_terrainService = terrainService;
		_eventBus = eventBus;
		_blockService = blockService;
	}

	public void Load()
	{
		InitializeArrays();
		_eventBus.Register(this);
	}

	public ReadOnlyList<RangedEffect> GetEffectsAffectingCoordinates(Vector2Int coordinates)
	{
		if (!_terrainService.Contains(coordinates))
		{
			return EmptyEffects.AsReadOnlyList();
		}
		return _rangedEffects[coordinates.x, coordinates.y].ActiveEffects;
	}

	[OnEvent]
	public void OnEnteredUnfinishedState(EnteredUnfinishedStateEvent enteredUnfinishedStateEvent)
	{
		BlockObject blockObject = enteredUnfinishedStateEvent.BlockObject;
		if (blockObject.HasComponent<UnfinishedEffectReceivingBuildingSpec>())
		{
			SetExistingAppliersToEnterable(blockObject.GetComponent<Enterable>(), add: true);
		}
	}

	[OnEvent]
	public void OnExitedUnfinishedState(ExitedUnfinishedStateEvent exitedUnfinishedStateEvent)
	{
		BlockObject blockObject = exitedUnfinishedStateEvent.BlockObject;
		if (blockObject.HasComponent<UnfinishedEffectReceivingBuildingSpec>())
		{
			SetExistingAppliersToEnterable(blockObject.GetComponent<Enterable>(), add: false);
		}
	}

	[OnEvent]
	public void OnEnteredFinishedState(EnteredFinishedStateEvent enteredFinishedStateEvent)
	{
		BlockObject blockObject = enteredFinishedStateEvent.BlockObject;
		SetExistingAppliersToEnterable(blockObject.GetComponent<Enterable>(), add: true);
	}

	[OnEvent]
	public void OnExitedFinishedState(ExitedFinishedStateEvent exitedFinishedStateEvent)
	{
		BlockObject blockObject = exitedFinishedStateEvent.BlockObject;
		SetExistingAppliersToEnterable(blockObject.GetComponent<Enterable>(), add: false);
	}

	public void SetApplier(RangedEffectApplier rangedEffectApplier)
	{
		SetApplier(rangedEffectApplier, add: true);
	}

	public void UnsetApplier(RangedEffectApplier rangedEffectApplier)
	{
		SetApplier(rangedEffectApplier, add: false);
	}

	private void SetApplier(RangedEffectApplier rangedEffectApplier, bool add)
	{
		foreach (Vector2Int item in rangedEffectApplier.EffectAreaCoords())
		{
			if (_terrainService.Contains(item))
			{
				AddApplierToExistingEnterablesAt(item, rangedEffectApplier, add);
				SetApplierAt(item, rangedEffectApplier, add);
			}
		}
	}

	private void SetApplierAt(Vector2Int coordinates, RangedEffectApplier rangedEffectApplier, bool add)
	{
		RangedEffects effectsAtCoordinates = GetEffectsAtCoordinates(coordinates);
		if (add)
		{
			effectsAtCoordinates.Add(rangedEffectApplier);
		}
		else
		{
			effectsAtCoordinates.Remove(rangedEffectApplier);
		}
	}

	private void SetExistingAppliersToEnterable(Enterable enterable, bool add)
	{
		if (!enterable)
		{
			return;
		}
		RangedEffectsAffectingEnterable component = enterable.GetComponent<RangedEffectsAffectingEnterable>();
		foreach (RangedEffectApplier item in GetExistingAppliersAffectingEnterable(enterable).ToHashSet())
		{
			if (add)
			{
				component.Add(item);
			}
			else
			{
				component.Remove(item);
			}
		}
	}

	private IEnumerable<RangedEffectApplier> GetExistingAppliersAffectingEnterable(Enterable enterable)
	{
		IEnumerable<Vector3Int> occupiedCoordinates = enterable.GetComponent<BlockObject>().PositionedBlocks.GetOccupiedCoordinates();
		foreach (Vector3Int item in occupiedCoordinates)
		{
			RangedEffects effectsAtCoordinates = GetEffectsAtCoordinates(item.XY());
			foreach (RangedEffectApplier rangedEffectApplier in effectsAtCoordinates.RangedEffectAppliers)
			{
				yield return rangedEffectApplier;
			}
		}
	}

	private void AddApplierToExistingEnterablesAt(Vector2Int coordinates, RangedEffectApplier rangedEffectApplier, bool add)
	{
		for (int i = 0; i <= _blockService.Size.z; i++)
		{
			Vector3Int coordinates2 = new Vector3Int(coordinates.x, coordinates.y, i);
			foreach (BlockObject item in _blockService.GetObjectsAt(coordinates2))
			{
				if (item.HasComponent<BuildingSpec>() && IsValid(item))
				{
					AddApplierToEnterable(rangedEffectApplier, item, add);
				}
			}
		}
	}

	private static void AddApplierToEnterable(RangedEffectApplier rangedEffectApplier, BlockObject building, bool add)
	{
		RangedEffectsAffectingEnterable component = building.GetComponent<RangedEffectsAffectingEnterable>();
		if ((bool)component)
		{
			if (add)
			{
				component.Add(rangedEffectApplier);
			}
			else
			{
				component.Remove(rangedEffectApplier);
			}
		}
	}

	private RangedEffects GetEffectsAtCoordinates(Vector2Int coordinates)
	{
		return _rangedEffects[coordinates.x, coordinates.y];
	}

	private static bool IsValid(BlockObject blockObject)
	{
		if (!blockObject.IsFinished)
		{
			return blockObject.HasComponent<UnfinishedEffectReceivingBuildingSpec>();
		}
		return true;
	}

	private void InitializeArrays()
	{
		_rangedEffects = new RangedEffects[_terrainService.Size.x, _terrainService.Size.y];
		for (int i = 0; i <= _rangedEffects.GetUpperBound(0); i++)
		{
			for (int j = 0; j <= _rangedEffects.GetUpperBound(1); j++)
			{
				_rangedEffects[i, j] = new RangedEffects();
			}
		}
	}
}
