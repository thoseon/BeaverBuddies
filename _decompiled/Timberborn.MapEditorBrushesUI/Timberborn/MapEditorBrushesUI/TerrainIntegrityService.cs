using System;
using System.Collections.Generic;
using Timberborn.BlockSystem;
using Timberborn.BlueprintSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.SelectionSystem;
using Timberborn.SingletonSystem;
using Timberborn.TerrainPhysics;
using Timberborn.TerrainSystem;
using Timberborn.TerrainSystemRendering;
using UnityEngine;

namespace Timberborn.MapEditorBrushesUI;

public class TerrainIntegrityService : ILoadableSingleton
{
	private readonly ITerrainPhysicsService _terrainPhysicsService;

	private readonly TerrainHighlightingService _terrainHighlightingService;

	private readonly RollingHighlighter _rollingHighlighter;

	private readonly EntityService _entityService;

	private readonly ITerrainService _terrainService;

	private readonly IBlockService _blockService;

	private readonly ISpecService _specService;

	private BrushColorSpec _brushColorSpec;

	private readonly List<Vector3Int> _coordinatesCache = new List<Vector3Int>();

	private readonly HashSet<BlockObject> _conflictingBlockObjects = new HashSet<BlockObject>();

	private readonly HashSet<BlockObject> _blockObjectCheckOutput = new HashSet<BlockObject>();

	private readonly HashSet<Vector3Int> _conflictingTerrain = new HashSet<Vector3Int>();

	private readonly HashSet<Vector3Int> _terrainCheckOutput = new HashSet<Vector3Int>();

	private bool _isHighlighted;

	public event EventHandler<bool> HighlightChanged;

	public TerrainIntegrityService(ITerrainPhysicsService terrainPhysicsService, TerrainHighlightingService terrainHighlightingService, RollingHighlighter rollingHighlighter, EntityService entityService, ITerrainService terrainService, IBlockService blockService, ISpecService specService)
	{
		_terrainPhysicsService = terrainPhysicsService;
		_terrainHighlightingService = terrainHighlightingService;
		_rollingHighlighter = rollingHighlighter;
		_entityService = entityService;
		_terrainService = terrainService;
		_blockService = blockService;
		_specService = specService;
	}

	public void Load()
	{
		_brushColorSpec = _specService.GetSingleSpec<BrushColorSpec>();
	}

	public void RemoveViolatingElements(IEnumerable<Vector3Int> blockObjectChanges, IEnumerable<Vector3Int> integrityChanges)
	{
		RemoveConflictingObjects(blockObjectChanges);
		CleanupCache();
		_conflictingTerrain.AddRange(_terrainCheckOutput);
		_terrainCheckOutput.Clear();
		RemoveStackedTerrainAndObjects(integrityChanges);
		CleanupCache();
	}

	public void HighlightViolatingElements(IEnumerable<Vector3Int> blockObjectChanges, IEnumerable<Vector3Int> integrityChanges)
	{
		ClearHighlight();
		CollectConflictingObjects(blockObjectChanges);
		CollectConflictingTerrainAndBlockObjectStack(integrityChanges);
		if (_conflictingBlockObjects.Count > 0)
		{
			_rollingHighlighter.HighlightPrimary(_conflictingBlockObjects, _brushColorSpec.Objects);
			_isHighlighted = true;
		}
		if (_conflictingTerrain.Count > 0)
		{
			_terrainHighlightingService.UpdateHighlight(_conflictingTerrain);
			_isHighlighted = true;
		}
		HighlightChanged?.Invoke(this, _isHighlighted);
		CleanupCache();
	}

	public void ClearHighlight()
	{
		if (_isHighlighted)
		{
			_terrainHighlightingService.ClearHighlight();
			_rollingHighlighter.UnhighlightAllPrimary();
			_isHighlighted = false;
			HighlightChanged?.Invoke(this, e: false);
		}
	}

	private void RemoveConflictingObjects(IEnumerable<Vector3Int> blocks)
	{
		CollectConflictingObjects(blocks);
		_terrainPhysicsService.GetTerrainAndBlockObjectStack(_conflictingBlockObjects, _terrainCheckOutput, _blockObjectCheckOutput);
		_conflictingBlockObjects.AddRange(_blockObjectCheckOutput);
		_blockObjectCheckOutput.Clear();
		foreach (BlockObject conflictingBlockObject in _conflictingBlockObjects)
		{
			_entityService.Delete(conflictingBlockObject);
		}
	}

	private void CollectConflictingObjects(IEnumerable<Vector3Int> blocks)
	{
		foreach (Vector3Int block in blocks)
		{
			if (_blockService.AnyObjectAt(block))
			{
				_conflictingBlockObjects.AddRange(_blockService.GetObjectsAt(block));
			}
			if (_blockService.BlockNeedsGroundBelow(block.Above()))
			{
				_conflictingBlockObjects.AddRange(_blockService.GetObjectsAt(block.Above()));
			}
		}
	}

	private void RemoveStackedTerrainAndObjects(IEnumerable<Vector3Int> blocks)
	{
		CollectConflictingTerrainAndBlockObjectStack(blocks);
		foreach (BlockObject conflictingBlockObject in _conflictingBlockObjects)
		{
			_entityService.Delete(conflictingBlockObject);
		}
		foreach (Vector3Int item in _conflictingTerrain)
		{
			_terrainService.UnsetTerrain(item);
		}
	}

	private void CollectConflictingTerrainAndBlockObjectStack(IEnumerable<Vector3Int> blocks)
	{
		_coordinatesCache.AddRange(blocks);
		_terrainPhysicsService.GetTerrainAndBlockObjectStack(_coordinatesCache, _conflictingBlockObjects, _conflictingTerrain, _blockObjectCheckOutput);
		_conflictingBlockObjects.AddRange(_blockObjectCheckOutput);
		_blockObjectCheckOutput.Clear();
		_coordinatesCache.Clear();
	}

	private void CleanupCache()
	{
		_conflictingBlockObjects.Clear();
		_conflictingTerrain.Clear();
	}
}
