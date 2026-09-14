using System.Collections.Generic;
using System.Linq;
using Timberborn.AreaSelectionSystem;
using Timberborn.AreaSelectionSystemUI;
using Timberborn.BlueprintSystem;
using Timberborn.Brushes;
using Timberborn.Common;
using Timberborn.InputSystem;
using Timberborn.LevelVisibilitySystem;
using Timberborn.Localization;
using Timberborn.MapEditorConstructionGuidelinesUI;
using Timberborn.MapStateSystem;
using Timberborn.Rendering;
using Timberborn.SingletonSystem;
using Timberborn.TerrainPhysics;
using Timberborn.TerrainQueryingSystem;
using Timberborn.TerrainSystem;
using Timberborn.ToolSystem;
using Timberborn.ToolSystemUI;
using Timberborn.UndoSystem;
using UnityEngine;

namespace Timberborn.MapEditorBrushesUI;

public class SculptingTerrainBrushTool : ITool, IToolDescriptor, IInputProcessor, ILoadableSingleton, IBrushWithDirection, IBrushWithGuidelines
{
	private static readonly string TitleLocKey = "MapEditor.Brush.SculptingTerrain";

	private static readonly float MarkerYOffset = 0.02f;

	private readonly InputService _inputService;

	private readonly ITerrainService _terrainService;

	private readonly SculptingTerrainPicker _sculptingTerrainPicker;

	private readonly TerrainPicker _terrainPicker;

	private readonly MarkerDrawerFactory _markerDrawerFactory;

	private readonly ITerrainPhysicsService _terrainPhysicsService;

	private readonly ILevelVisibilityService _levelVisibilityService;

	private readonly TerrainIntegrityService _terrainIntegrityService;

	private readonly ILoc _loc;

	private readonly MeasurableAreaDrawer _measurableAreaDrawer;

	private readonly IUndoRegistry _undoRegistry;

	private readonly MapSize _mapSize;

	private readonly ISpecService _specService;

	private BrushColorSpec _brushColorSpec;

	private MeshDrawer _smallMarkerDrawer;

	private MeshDrawer _largeMarkerDrawer;

	private ToolDescription _toolDescription;

	private readonly HashSet<Vector3Int> _candidateBlocks = new HashSet<Vector3Int>();

	private readonly HashSet<Vector3Int> _blocksToApply = new HashSet<Vector3Int>();

	public bool Increase { get; set; } = true;

	public bool Inverse { get; set; }

	public bool IsIncreasing
	{
		get
		{
			if (!Increase || Inverse)
			{
				if (!Increase)
				{
					return Inverse;
				}
				return false;
			}
			return true;
		}
	}

	public SculptingTerrainBrushTool(InputService inputService, ITerrainService terrainService, SculptingTerrainPicker sculptingTerrainPicker, TerrainPicker terrainPicker, MarkerDrawerFactory markerDrawerFactory, ITerrainPhysicsService terrainPhysicsService, ILevelVisibilityService levelVisibilityService, TerrainIntegrityService terrainIntegrityService, ILoc loc, MeasurableAreaDrawer measurableAreaDrawer, IUndoRegistry undoRegistry, MapSize mapSize, ISpecService specService)
	{
		_inputService = inputService;
		_terrainService = terrainService;
		_sculptingTerrainPicker = sculptingTerrainPicker;
		_terrainPicker = terrainPicker;
		_markerDrawerFactory = markerDrawerFactory;
		_terrainPhysicsService = terrainPhysicsService;
		_levelVisibilityService = levelVisibilityService;
		_terrainIntegrityService = terrainIntegrityService;
		_loc = loc;
		_measurableAreaDrawer = measurableAreaDrawer;
		_undoRegistry = undoRegistry;
		_mapSize = mapSize;
		_specService = specService;
	}

	public void Load()
	{
		_brushColorSpec = _specService.GetSingleSpec<BrushColorSpec>();
		_toolDescription = new ToolDescription.Builder(_loc.T(TitleLocKey)).Build();
		_smallMarkerDrawer = _markerDrawerFactory.CreateSmallBlockTileDrawer();
		_largeMarkerDrawer = _markerDrawerFactory.CreateLargeBlockTileDrawer();
	}

	public bool ProcessInput()
	{
		if (!IsIncreasing)
		{
			return _sculptingTerrainPicker.PickTerrainAreaToRemove(DrawPreview, ApplyChanges);
		}
		return _sculptingTerrainPicker.PickTerrainAreaToAdd(DrawPreview, ApplyChanges);
	}

	public void Enter()
	{
		_inputService.AddInputProcessor(this);
	}

	public void Exit()
	{
		_inputService.RemoveInputProcessor(this);
		_terrainIntegrityService.ClearHighlight();
		_sculptingTerrainPicker.Reset();
	}

	public ToolDescription DescribeTool()
	{
		return _toolDescription;
	}

	private void ApplyChanges(IEnumerable<Vector3Int> pickedBlocks, Ray ray)
	{
		if (_terrainPicker.PickTerrainCoordinates(ray).HasValue)
		{
			UpdateBlocksCache(pickedBlocks);
			TerrainIntegrityService terrainIntegrityService = _terrainIntegrityService;
			HashSet<Vector3Int> blocksToApply = _blocksToApply;
			IEnumerable<Vector3Int> integrityChanges;
			if (!IsIncreasing)
			{
				IEnumerable<Vector3Int> blocksToApply2 = _blocksToApply;
				integrityChanges = blocksToApply2;
			}
			else
			{
				integrityChanges = Enumerable.Empty<Vector3Int>();
			}
			terrainIntegrityService.RemoveViolatingElements(blocksToApply, integrityChanges);
			foreach (Vector3Int item in _blocksToApply)
			{
				if (IsIncreasing)
				{
					_terrainService.SetTerrain(item);
				}
				else
				{
					_terrainService.UnsetTerrain(item);
				}
			}
		}
		ClearBlocksCache();
		_undoRegistry.CommitStack();
	}

	private void DrawPreview(IEnumerable<Vector3Int> pickedBlocks, Ray ray)
	{
		_terrainIntegrityService.ClearHighlight();
		if (_terrainPicker.PickTerrainCoordinates(ray).HasValue)
		{
			UpdateBlocksCache(pickedBlocks);
			_measurableAreaDrawer.AddMeasurableCoordinates(_blocksToApply);
			TerrainIntegrityService terrainIntegrityService = _terrainIntegrityService;
			HashSet<Vector3Int> blocksToApply = _blocksToApply;
			IEnumerable<Vector3Int> integrityChanges;
			if (!IsIncreasing)
			{
				IEnumerable<Vector3Int> blocksToApply2 = _blocksToApply;
				integrityChanges = blocksToApply2;
			}
			else
			{
				integrityChanges = Enumerable.Empty<Vector3Int>();
			}
			terrainIntegrityService.HighlightViolatingElements(blocksToApply, integrityChanges);
			foreach (Vector3Int item in _blocksToApply)
			{
				if (IsIncreasing)
				{
					_smallMarkerDrawer.DrawAtCoordinates(item, MarkerYOffset, _brushColorSpec.Positive);
				}
				else
				{
					_largeMarkerDrawer.DrawAtCoordinates(item, MarkerYOffset, _brushColorSpec.Negative);
				}
			}
		}
		ClearBlocksCache();
	}

	private void UpdateBlocksCache(IEnumerable<Vector3Int> pickedBlocks)
	{
		CollectCandidateBlocks(pickedBlocks);
		CollectBlocksToApply();
	}

	private void CollectCandidateBlocks(IEnumerable<Vector3Int> pickedBlocks)
	{
		foreach (Vector3Int pickedBlock in pickedBlocks)
		{
			if (IsValidCandidateBlock(pickedBlock))
			{
				_candidateBlocks.Add(pickedBlock);
			}
		}
	}

	private bool IsValidCandidateBlock(Vector3Int block)
	{
		bool flag = _terrainService.Underground(block);
		if (((IsIncreasing && !flag) || (!IsIncreasing & flag)) && _terrainService.Contains(block) && block.z < _levelVisibilityService.MaxVisibleLevel + 1)
		{
			return block.z < _mapSize.MaxMapEditorTerrainHeight;
		}
		return false;
	}

	private void CollectBlocksToApply()
	{
		if (IsIncreasing)
		{
			_terrainPhysicsService.GetValidTerrainToAdd(_candidateBlocks, _blocksToApply);
		}
		else
		{
			_blocksToApply.AddRange(_candidateBlocks);
		}
	}

	private void ClearBlocksCache()
	{
		_candidateBlocks.Clear();
		_blocksToApply.Clear();
	}
}
