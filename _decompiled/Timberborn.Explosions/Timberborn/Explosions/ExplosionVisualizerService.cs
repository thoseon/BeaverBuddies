using System;
using System.Collections.Generic;
using Timberborn.BlockSystem;
using Timberborn.BlueprintSystem;
using Timberborn.SelectionSystem;
using Timberborn.SingletonSystem;
using Timberborn.TerrainSystemRendering;
using Timberborn.ToolSystem;
using UnityEngine;

namespace Timberborn.Explosions;

internal class ExplosionVisualizerService : ILoadableSingleton
{
	private readonly TerrainHighlightingService _terrainHighlightingService;

	private readonly ExplosionOutcomeGatherer _explosionOutcomeGatherer;

	private readonly RollingHighlighter _rollingHighlighter;

	private readonly ISpecService _specService;

	private readonly EventBus _eventBus;

	private readonly HashSet<Vector3Int> _affectedTiles = new HashSet<Vector3Int>();

	private readonly HashSet<Vector3Int> _affectedTerrain = new HashSet<Vector3Int>();

	private readonly HashSet<BlockObject> _affectedObjects = new HashSet<BlockObject>();

	private ExplosionVisualizerSpec _spec;

	private UnstableCore _unstableCore;

	private BlockObject _blockObject;

	public ExplosionVisualizerService(TerrainHighlightingService terrainHighlightingService, ExplosionOutcomeGatherer explosionOutcomeGatherer, RollingHighlighter rollingHighlighter, ISpecService specService, EventBus eventBus)
	{
		_terrainHighlightingService = terrainHighlightingService;
		_explosionOutcomeGatherer = explosionOutcomeGatherer;
		_rollingHighlighter = rollingHighlighter;
		_specService = specService;
		_eventBus = eventBus;
	}

	public void Load()
	{
		_eventBus.Register(this);
		_spec = _specService.GetSingleSpec<ExplosionVisualizerSpec>();
	}

	[OnEvent]
	public void OnToolExited(ToolExitedEvent toolExitedEvent)
	{
		if (_unstableCore != null)
		{
			ClearSelected(null);
		}
	}

	public void UpdateHighlight(UnstableCore unstableCore)
	{
		ClearSelected(_unstableCore);
		_unstableCore = unstableCore;
		_blockObject = _unstableCore.GetComponent<BlockObject>();
		Highlight();
		_unstableCore.ExplosionRadiusChanged += OnExplosionRadiusChanged;
	}

	public void ClearSelected(UnstableCore unstableCore)
	{
		if (_unstableCore != null && (unstableCore == null || unstableCore == _unstableCore))
		{
			ClearHighlight();
			_unstableCore.ExplosionRadiusChanged -= OnExplosionRadiusChanged;
			_unstableCore = null;
			_blockObject = null;
		}
	}

	private void OnExplosionRadiusChanged(object sender, EventArgs e)
	{
		Highlight();
	}

	private void Highlight()
	{
		ClearHighlight();
		_explosionOutcomeGatherer.GetAllAffectedTerrainAndObjects(_unstableCore, _affectedTiles, _affectedTerrain, _affectedObjects);
		_affectedObjects.Remove(_blockObject);
		_terrainHighlightingService.UpdateHighlight(_affectedTerrain);
		_rollingHighlighter.HighlightPrimary(_affectedObjects, _spec.ObjectHighlightColor);
		_affectedTiles.Clear();
		_affectedTerrain.Clear();
		_affectedObjects.Clear();
	}

	private void ClearHighlight()
	{
		_rollingHighlighter.UnhighlightAllPrimary();
		_terrainHighlightingService.ClearHighlight();
	}
}
