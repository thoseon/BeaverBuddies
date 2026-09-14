using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.AreaSelectionSystem;
using Timberborn.AreaSelectionSystemUI;
using Timberborn.BlockObjectPickingSystem;
using Timberborn.BlockSystem;
using Timberborn.BlueprintSystem;
using Timberborn.CoreUI;
using Timberborn.EntitySystem;
using Timberborn.InputSystem;
using Timberborn.LevelVisibilitySystem;
using Timberborn.SingletonSystem;
using Timberborn.TerrainPhysics;
using Timberborn.TerrainSystemRendering;
using Timberborn.ToolSystem;
using Timberborn.ToolSystemUI;
using Timberborn.UndoSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.BlockObjectTools;

public abstract class BlockObjectDeletionTool<T> : ITool, IToolDescriptor, ILoadableSingleton, IInputProcessor
{
	private static readonly string SkipDeleteConfirmationKey = "SkipDeleteConfirmation";

	private readonly AreaBlockObjectAndTerrainPicker _areaBlockObjectAndTerrainPicker;

	private readonly InputService _inputService;

	private readonly EntityService _entityService;

	private readonly BlockObjectSelectionDrawerFactory _blockObjectSelectionDrawerFactory;

	private readonly CursorService _cursorService;

	private readonly BlockObjectModelBlockadeIgnorer _blockObjectModelBlockadeIgnorer;

	private readonly ISpecService _specService;

	private readonly ILevelVisibilityService _levelVisibilityService;

	private readonly DialogBoxShower _dialogBoxShower;

	private readonly TerrainDestroyer _terrainDestroyer;

	private readonly TerrainHighlightingService _terrainHighlightingService;

	private readonly IUndoRegistry _undoRegistry;

	private readonly List<BlockObject> _temporaryBlockObjects = new List<BlockObject>();

	private readonly List<Vector3Int> _temporaryTerrainCoords = new List<Vector3Int>();

	private BlockObjectDeletionToolSpec _blockObjectDeletionToolSpec;

	private BlockObjectSelectionDrawer _blockObjectSelectionDrawer;

	private bool _skipConfirmation;

	private bool _paused;

	private int _maxVisibleLevelToReset;

	protected abstract string ToolPromptLocKey { get; }

	protected abstract string CursorKey { get; }

	protected BlockObjectDeletionTool(InputService inputService, AreaBlockObjectAndTerrainPicker areaBlockObjectAndTerrainPicker, EntityService entityService, BlockObjectSelectionDrawerFactory blockObjectSelectionDrawerFactory, CursorService cursorService, BlockObjectModelBlockadeIgnorer blockObjectModelBlockadeIgnorer, ISpecService specService, ILevelVisibilityService levelVisibilityService, DialogBoxShower dialogBoxShower, TerrainDestroyer terrainDestroyer, TerrainHighlightingService terrainHighlightingService, IUndoRegistry undoRegistry)
	{
		_inputService = inputService;
		_areaBlockObjectAndTerrainPicker = areaBlockObjectAndTerrainPicker;
		_entityService = entityService;
		_blockObjectSelectionDrawerFactory = blockObjectSelectionDrawerFactory;
		_cursorService = cursorService;
		_blockObjectModelBlockadeIgnorer = blockObjectModelBlockadeIgnorer;
		_specService = specService;
		_levelVisibilityService = levelVisibilityService;
		_dialogBoxShower = dialogBoxShower;
		_terrainDestroyer = terrainDestroyer;
		_terrainHighlightingService = terrainHighlightingService;
		_undoRegistry = undoRegistry;
	}

	public void Load()
	{
		_blockObjectDeletionToolSpec = _specService.GetSingleSpec<BlockObjectDeletionToolSpec>();
		_blockObjectSelectionDrawer = _blockObjectSelectionDrawerFactory.Create(_blockObjectDeletionToolSpec.DeletedObjectHighlightColor, _blockObjectDeletionToolSpec.DeletedAreaTileColor, _blockObjectDeletionToolSpec.DeletedAreaSideColor);
	}

	public bool ProcessInput()
	{
		if (!_paused)
		{
			_skipConfirmation = _inputService.IsKeyHeld(SkipDeleteConfirmationKey) || _undoRegistry.UndoAllowed;
			return _areaBlockObjectAndTerrainPicker.PickBlockObjectsAndTerrain<T>(PreviewCallback, ActionCallback, ShowNoneCallback, IsBlockObjectValid);
		}
		return false;
	}

	public virtual void Enter()
	{
		_inputService.AddInputProcessor(this);
		_cursorService.SetCursor(CursorKey);
	}

	public virtual void Exit()
	{
		_cursorService.ResetCursor();
		_areaBlockObjectAndTerrainPicker.Reset();
		_blockObjectSelectionDrawer.StopDrawing();
		_inputService.RemoveInputProcessor(this);
		_terrainHighlightingService.ClearHighlight();
	}

	public abstract ToolDescription DescribeTool();

	protected virtual VisualElement GetDialogBoxContent(IEnumerable<BlockObject> blockObjects)
	{
		return null;
	}

	protected virtual void PostPreviewAction(IEnumerable<BlockObject> blockObjects)
	{
	}

	protected virtual void PreviewCallback(IEnumerable<BlockObject> blockObjects, IEnumerable<Vector3Int> terrainCoords, Vector3Int start, Vector3Int end, bool selectionStarted, bool selectingArea)
	{
		_temporaryBlockObjects.AddRange(blockObjects);
		_blockObjectSelectionDrawer.Draw(_temporaryBlockObjects, start, end, selectingArea);
		_terrainHighlightingService.UpdateHighlight(terrainCoords);
		PostPreviewAction(_temporaryBlockObjects);
		_temporaryBlockObjects.Clear();
	}

	protected virtual bool IsBlockObjectValid(BlockObject blockObject)
	{
		return true;
	}

	private void ShowNoneCallback()
	{
		_blockObjectSelectionDrawer.StopDrawing();
		_terrainHighlightingService.ClearHighlight();
	}

	private void ActionCallback(IEnumerable<BlockObject> blockObjects, IEnumerable<Vector3Int> terrainCoords, Vector3Int start, Vector3Int end, bool selectionStarted, bool selectingArea)
	{
		_temporaryBlockObjects.AddRange(blockObjects.OrderByDescending((BlockObject blockObject) => blockObject.Transform.position.y));
		_temporaryTerrainCoords.AddRange(terrainCoords);
		if (_temporaryBlockObjects.Count > 0)
		{
			if (_skipConfirmation)
			{
				DeleteBlockObjects();
				return;
			}
			_blockObjectSelectionDrawer.Draw(_temporaryBlockObjects, start, end, selectingArea);
			_blockObjectModelBlockadeIgnorer.IgnoreModelBlockades(_temporaryBlockObjects);
			_maxVisibleLevelToReset = _levelVisibilityService.MaxVisibleLevel;
			SetVisibleLayerToShowAllObjects();
			Pause();
			_dialogBoxShower.Create().SetLocalizedMessage(ToolPromptLocKey).SetConfirmButton(OnDeleteConfirmed)
				.SetCancelButton(OnDeleteCanceled)
				.SetOffset(new Vector2Int(0, -200))
				.AddContent(GetDialogBoxContent(_temporaryBlockObjects))
				.Show();
		}
	}

	private void SetVisibleLayerToShowAllObjects()
	{
		int num = 0;
		foreach (Vector3Int temporaryTerrainCoord in _temporaryTerrainCoords)
		{
			num = Math.Max(num, temporaryTerrainCoord.z);
		}
		foreach (BlockObject temporaryBlockObject in _temporaryBlockObjects)
		{
			foreach (Vector3Int allCoordinate in temporaryBlockObject.PositionedBlocks.GetAllCoordinates())
			{
				num = Math.Max(num, allCoordinate.z);
			}
		}
		if (num > _levelVisibilityService.MaxVisibleLevel)
		{
			_levelVisibilityService.SetMaxVisibleLevel(num);
		}
	}

	private void OnDeleteConfirmed()
	{
		DeleteBlockObjects();
		Unpause();
		_levelVisibilityService.SetMaxVisibleLevel(_maxVisibleLevelToReset);
		_blockObjectModelBlockadeIgnorer.Clear();
		_terrainHighlightingService.ClearHighlight();
		_areaBlockObjectAndTerrainPicker.Reset();
	}

	private void OnDeleteCanceled()
	{
		Unpause();
		_levelVisibilityService.SetMaxVisibleLevel(_maxVisibleLevelToReset);
		_blockObjectModelBlockadeIgnorer.UnignoreModelBlockades();
		_temporaryBlockObjects.Clear();
		_temporaryTerrainCoords.Clear();
		_terrainHighlightingService.ClearHighlight();
		_areaBlockObjectAndTerrainPicker.Reset();
	}

	private void DeleteBlockObjects()
	{
		foreach (BlockObject temporaryBlockObject in _temporaryBlockObjects)
		{
			if ((bool)temporaryBlockObject)
			{
				_entityService.Delete(temporaryBlockObject);
			}
		}
		foreach (Vector3Int temporaryTerrainCoord in _temporaryTerrainCoords)
		{
			_terrainDestroyer.DestroyTerrain(temporaryTerrainCoord);
		}
		_temporaryTerrainCoords.Clear();
		_temporaryBlockObjects.Clear();
		_undoRegistry.CommitStack();
	}

	private void Pause()
	{
		_paused = true;
		_cursorService.ResetCursor();
	}

	private void Unpause()
	{
		_paused = false;
		_cursorService.SetCursor(CursorKey);
	}
}
