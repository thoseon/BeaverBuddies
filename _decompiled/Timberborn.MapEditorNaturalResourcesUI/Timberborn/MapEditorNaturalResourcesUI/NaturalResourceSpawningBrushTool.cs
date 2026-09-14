using System.Collections.Generic;
using Timberborn.BlueprintSystem;
using Timberborn.Brushes;
using Timberborn.Common;
using Timberborn.InputSystem;
using Timberborn.Localization;
using Timberborn.MapEditorConstructionGuidelinesUI;
using Timberborn.MapEditorNaturalResources;
using Timberborn.NaturalResources;
using Timberborn.Rendering;
using Timberborn.SingletonSystem;
using Timberborn.ToolSystem;
using Timberborn.ToolSystemUI;
using Timberborn.UndoSystem;
using UnityEngine;

namespace Timberborn.MapEditorNaturalResourcesUI;

public class NaturalResourceSpawningBrushTool : ITool, IToolDescriptor, IInputProcessor, ILoadableSingleton, IBrushWithSize, IBrushWithShape, IBrushWithGuidelines
{
	private static readonly string TitleLocKey = "MapEditor.Brush.NaturalResourceSpawning";

	private static readonly float MarkerYOffset = 0.02f;

	private readonly InputService _inputService;

	private readonly NaturalResourceSpawner _naturalResourceSpawner;

	private readonly BrushProbabilityMap _brushProbabilityMap;

	private readonly MarkerDrawerFactory _markerDrawerFactory;

	private readonly ILoc _loc;

	private readonly NaturalResourceLayerService _naturalResourceLayerService;

	private readonly NaturalResourceBrushIterator _naturalResourceBrushIterator;

	private readonly IUndoRegistry _undoRegistry;

	private readonly ISpecService _specService;

	private MeshDrawer _meshDrawer;

	private ToolDescription _toolDescription;

	private HashSet<SpawnableResource> _enabledSpawnableResources;

	public int BrushSize { get; set; } = 3;

	public BrushShape BrushShape { get; set; }

	public float Density { get; set; } = 1f;

	public bool RandomizeYieldGrowth => _naturalResourceSpawner.RandomizeYieldGrowth;

	public NaturalResourceSpawningBrushTool(InputService inputService, NaturalResourceSpawner naturalResourceSpawner, BrushProbabilityMap brushProbabilityMap, MarkerDrawerFactory markerDrawerFactory, ILoc loc, NaturalResourceLayerService naturalResourceLayerService, NaturalResourceBrushIterator naturalResourceBrushIterator, IUndoRegistry undoRegistry, ISpecService specService)
	{
		_inputService = inputService;
		_naturalResourceSpawner = naturalResourceSpawner;
		_brushProbabilityMap = brushProbabilityMap;
		_markerDrawerFactory = markerDrawerFactory;
		_loc = loc;
		_naturalResourceLayerService = naturalResourceLayerService;
		_naturalResourceBrushIterator = naturalResourceBrushIterator;
		_undoRegistry = undoRegistry;
		_specService = specService;
	}

	public void Load()
	{
		InitializeToolDescription();
		NaturalResourceBrushSpec singleSpec = _specService.GetSingleSpec<NaturalResourceBrushSpec>();
		_meshDrawer = _markerDrawerFactory.CreateTileDrawer(singleSpec.SpawnTileColor);
		InitializeEnabledTypes(singleSpec.DefaultNaturalResourceId);
	}

	public bool ProcessInput()
	{
		ProcessBrush();
		if (!_inputService.MainMouseButtonHeld)
		{
			_undoRegistry.CommitStack();
		}
		return false;
	}

	public void Enter()
	{
		_inputService.AddInputProcessor(this);
	}

	public void Exit()
	{
		_inputService.RemoveInputProcessor(this);
		_naturalResourceBrushIterator.Reset();
		_undoRegistry.CommitStack();
	}

	public ToolDescription DescribeTool()
	{
		return _toolDescription;
	}

	public void EnableSpawnableResource(SpawnableResource id)
	{
		_enabledSpawnableResources.Add(id);
	}

	public void DisableSpawnableResource(SpawnableResource id)
	{
		_enabledSpawnableResources.Remove(id);
	}

	public bool IsNaturalResourceEnabled(SpawnableResource id)
	{
		return _enabledSpawnableResources.Contains(id);
	}

	public void SwitchRandomizeYieldGrowth(bool state)
	{
		_naturalResourceSpawner.RandomizeYieldGrowth = state;
	}

	private void InitializeEnabledTypes(string defaultType)
	{
		SpawnableResource item = new SpawnableResource(defaultType, isSeedling: false);
		_enabledSpawnableResources = new HashSet<SpawnableResource> { item };
	}

	private void ProcessBrush()
	{
		foreach (Vector3Int item in _naturalResourceBrushIterator.Iterate(BrushSize, BrushShape))
		{
			if (_inputService.MainMouseButtonHeld && !_inputService.MouseOverUI)
			{
				ProcessClickedTile(item);
			}
			_meshDrawer.DrawAtCoordinates(item, MarkerYOffset);
		}
	}

	private void ProcessClickedTile(Vector3Int coords3D)
	{
		_naturalResourceLayerService.Enable();
		if (!_enabledSpawnableResources.IsEmpty() && _brushProbabilityMap.TestProbabilityAtCoordinates(coords3D.XY(), Density))
		{
			_naturalResourceSpawner.Spawn(_enabledSpawnableResources, coords3D);
		}
	}

	private void InitializeToolDescription()
	{
		_toolDescription = new ToolDescription.Builder(_loc.T(TitleLocKey)).Build();
	}
}
