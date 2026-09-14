using System.Collections.Generic;
using Timberborn.BlueprintSystem;
using Timberborn.Brushes;
using Timberborn.Common;
using Timberborn.CursorToolSystem;
using Timberborn.InputSystem;
using Timberborn.Rendering;
using Timberborn.SingletonSystem;
using Timberborn.TickSystem;
using Timberborn.ToolSystem;
using Timberborn.ToolSystemUI;
using Timberborn.WaterSystem;
using UnityEngine;

namespace Timberborn.WaterBrushesUI;

public class WaterHeightBrushTool : IDevModeTool, ITool, IWaterIgnoringTool, IInputProcessor, ITickableSingleton, ILoadableSingleton, IBrushWithSize, IBrushWithShape
{
	private struct BrushWaterChange
	{
		public Vector3Int Coordinates { get; }

		public float WaterChange { get; }

		public bool IsContaminated { get; }

		public BrushWaterChange(Vector3Int coordinates, float waterChange, bool isContaminated)
		{
			Coordinates = coordinates;
			WaterChange = waterChange;
			IsContaminated = isContaminated;
		}
	}

	private static readonly float MarkerYOffset = 0.02f;

	private static readonly string RemoveWaterModifierKey = "RemoveWaterModifier";

	private static readonly string UseContaminationModifierKey = "UseContaminationModifier";

	private readonly InputService _inputService;

	private readonly IWaterService _waterService;

	private readonly BrushShapeIterator _brushShapeIterator;

	private readonly MarkerDrawerFactory _markerDrawerFactory;

	private readonly IThreadSafeWaterMap _threadSafeWaterMap;

	private readonly CursorCoordinatesPicker _cursorCoordinatesPicker;

	private readonly ISpecService _specService;

	private WaterHeightBrushSpec _waterHeightBrushSpec;

	private MeshDrawer _meshDrawer;

	private readonly Queue<BrushWaterChange> _waterChanges = new Queue<BrushWaterChange>();

	public int BrushSize { get; set; } = 1;

	public BrushShape BrushShape { get; set; }

	public bool IsDevMode => true;

	public WaterHeightBrushTool(InputService inputService, IWaterService waterService, BrushShapeIterator brushShapeIterator, MarkerDrawerFactory markerDrawerFactory, IThreadSafeWaterMap threadSafeWaterMap, CursorCoordinatesPicker cursorCoordinatesPicker, ISpecService specService)
	{
		_inputService = inputService;
		_waterService = waterService;
		_brushShapeIterator = brushShapeIterator;
		_markerDrawerFactory = markerDrawerFactory;
		_threadSafeWaterMap = threadSafeWaterMap;
		_cursorCoordinatesPicker = cursorCoordinatesPicker;
		_specService = specService;
	}

	public void Load()
	{
		_waterHeightBrushSpec = _specService.GetSingleSpec<WaterHeightBrushSpec>();
		_meshDrawer = _markerDrawerFactory.CreateTileDrawer();
	}

	public void Tick()
	{
		while (!_waterChanges.IsEmpty())
		{
			BrushWaterChange brushWaterChange = _waterChanges.Dequeue();
			Vector3Int coordinates = brushWaterChange.Coordinates;
			if (_threadSafeWaterMap.TryGetColumnFloor(coordinates, out var floor))
			{
				Vector3Int coordinates2 = new Vector3Int(coordinates.x, coordinates.y, floor);
				float waterChange = brushWaterChange.WaterChange;
				if (waterChange < 0f)
				{
					_waterService.RemoveCleanWater(coordinates2, 0f - waterChange);
					_waterService.RemoveContaminatedWater(coordinates2, 0f - waterChange);
				}
				else if (brushWaterChange.IsContaminated)
				{
					_waterService.AddContaminatedWater(coordinates2, waterChange);
				}
				else
				{
					_waterService.AddCleanWater(coordinates2, waterChange);
				}
			}
		}
	}

	public bool ProcessInput()
	{
		CursorCoordinates? cursorCoordinates = _cursorCoordinatesPicker.Pick();
		if (cursorCoordinates.HasValue)
		{
			CursorCoordinates valueOrDefault = cursorCoordinates.GetValueOrDefault();
			bool isRemoving = _inputService.IsKeyHeld(RemoveWaterModifierKey);
			bool isContaminating = _inputService.IsKeyHeld(UseContaminationModifierKey);
			if (_inputService.MainMouseButtonDown && !_inputService.MouseOverUI)
			{
				ApplyBrush(valueOrDefault.TileCoordinates, isRemoving, isContaminating);
			}
			DrawTileMarkers(valueOrDefault.TileCoordinates, isRemoving, isContaminating);
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
	}

	private void ApplyBrush(Vector3Int tileCoordinates, bool isRemoving, bool isContaminating)
	{
		foreach (Vector3Int affectedCoordinate in GetAffectedCoordinates(tileCoordinates))
		{
			float num = _threadSafeWaterMap.WaterHeightOrFloor(affectedCoordinate) % 1f;
			if (!isRemoving && num > 0.99f)
			{
				num = 0f;
			}
			float waterChange = (isRemoving ? (0f - num) : (1f - num));
			_waterChanges.Enqueue(new BrushWaterChange(affectedCoordinate, waterChange, isContaminating));
		}
	}

	private IEnumerable<Vector3Int> GetAffectedCoordinates(Vector3Int center)
	{
		IEnumerable<Vector3Int> enumerable = _brushShapeIterator.IterateShape(center, BrushSize, BrushShape);
		foreach (Vector3Int item in enumerable)
		{
			yield return new Vector3Int(item.x, item.y, center.z);
		}
	}

	private void DrawTileMarkers(Vector3Int center, bool isRemoving, bool isContaminating)
	{
		foreach (Vector3Int affectedCoordinate in GetAffectedCoordinates(center))
		{
			Color color = GetColor(isRemoving, isContaminating);
			_meshDrawer.DrawAtCoordinates(affectedCoordinate, MarkerYOffset, color);
		}
	}

	private Color GetColor(bool isRemoving, bool isContaminating)
	{
		if (isRemoving)
		{
			return _waterHeightBrushSpec.RemovingTileColor;
		}
		if (isContaminating)
		{
			return _waterHeightBrushSpec.ContaminatedTileColor;
		}
		return _waterHeightBrushSpec.AddingTileColor;
	}
}
