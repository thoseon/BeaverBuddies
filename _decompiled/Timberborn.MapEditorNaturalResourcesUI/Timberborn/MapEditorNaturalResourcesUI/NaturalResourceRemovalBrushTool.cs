using System.Collections.Generic;
using Timberborn.BlockSystem;
using Timberborn.BlueprintSystem;
using Timberborn.Brushes;
using Timberborn.EntitySystem;
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

public class NaturalResourceRemovalBrushTool : ITool, IToolDescriptor, IInputProcessor, ILoadableSingleton, IBrushWithSize, IBrushWithShape, IBrushWithGuidelines
{
	private static readonly string TitleLocKey = "MapEditor.Brush.NaturalResourceRemoval";

	private static readonly float MarkerYOffset = 0.02f;

	private readonly InputService _inputService;

	private readonly IBlockService _blockService;

	private readonly EntityService _entityService;

	private readonly MarkerDrawerFactory _markerDrawerFactory;

	private readonly ILoc _loc;

	private readonly NaturalResourceLayerService _naturalResourceLayerService;

	private readonly NaturalResourceBrushIterator _naturalResourceBrushIterator;

	private readonly IUndoRegistry _undoRegistry;

	private readonly ISpecService _specService;

	private MeshDrawer _meshDrawer;

	private ToolDescription _toolDescription;

	private readonly List<NaturalResource> _resourcesToDelete = new List<NaturalResource>();

	public int BrushSize { get; set; } = 3;

	public BrushShape BrushShape { get; set; }

	public NaturalResourceRemovalBrushTool(InputService inputService, IBlockService blockService, EntityService entityService, MarkerDrawerFactory markerDrawerFactory, ILoc loc, NaturalResourceLayerService naturalResourceLayerService, NaturalResourceBrushIterator naturalResourceBrushIterator, IUndoRegistry undoRegistry, ISpecService specService)
	{
		_inputService = inputService;
		_blockService = blockService;
		_entityService = entityService;
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
		_meshDrawer = _markerDrawerFactory.CreateTileDrawer(singleSpec.RemovalTileColor);
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

	private void ProcessBrush()
	{
		foreach (Vector3Int item in _naturalResourceBrushIterator.Iterate(BrushSize, BrushShape))
		{
			if (_inputService.MainMouseButtonHeld && !_inputService.MouseOverUI)
			{
				DeleteNaturalResourcesAt(item);
			}
			_meshDrawer.DrawAtCoordinates(item, MarkerYOffset);
		}
	}

	private void DeleteNaturalResourcesAt(Vector3Int coords3D)
	{
		_naturalResourceLayerService.Enable();
		_resourcesToDelete.AddRange(_blockService.GetObjectsWithComponentAt<NaturalResource>(coords3D));
		foreach (NaturalResource item in _resourcesToDelete)
		{
			_entityService.Delete(item);
		}
		_resourcesToDelete.Clear();
	}

	private void InitializeToolDescription()
	{
		_toolDescription = new ToolDescription.Builder(_loc.T(TitleLocKey)).Build();
	}
}
