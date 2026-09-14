using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.BlockObjectPickingSystem;
using Timberborn.BlockSystem;
using Timberborn.BlueprintSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.GridTraversing;
using Timberborn.SingletonSystem;
using Timberborn.TerrainQueryingSystem;
using Timberborn.TerrainSystem;
using UnityEngine;

namespace Timberborn.AreaSelectionSystem;

public class AreaPicker : ILoadableSingleton
{
	public delegate void IntAreaCallback(IEnumerable<Vector3Int> blocks, Ray ray);

	public delegate void BlockObjectAreaCallback(IEnumerable<Placement> placements);

	private readonly TerrainPicker _terrainPicker;

	private readonly BlockObjectPreviewPicker _blockObjectPreviewPicker;

	private readonly AreaSelectionController _areaSelectionController;

	private readonly StackableBlockService _stackableBlockService;

	private readonly ITerrainService _terrainService;

	private readonly AreaIterator _areaIterator;

	private readonly ISpecService _specService;

	private int _maxBlocks;

	private LineDirection _segmentedLineDirection;

	public AreaPicker(TerrainPicker terrainPicker, BlockObjectPreviewPicker blockObjectPreviewPicker, AreaSelectionController areaSelectionController, StackableBlockService stackableBlockService, ITerrainService terrainService, AreaIterator areaIterator, ISpecService specService)
	{
		_terrainPicker = terrainPicker;
		_blockObjectPreviewPicker = blockObjectPreviewPicker;
		_areaSelectionController = areaSelectionController;
		_stackableBlockService = stackableBlockService;
		_terrainService = terrainService;
		_areaIterator = areaIterator;
		_specService = specService;
	}

	public void Load()
	{
		_maxBlocks = _specService.GetSingleSpec<AreaPickersSpec>().AreaMaxBlocks;
	}

	public bool PickTerrainIntArea(IntAreaCallback previewCallback, IntAreaCallback actionCallback, Action showNoneCallback)
	{
		return _areaSelectionController.ProcessInput(delegate(Ray start, Ray end, bool _)
		{
			previewCallback(GetTerrainBlocks(start, end), start);
		}, delegate(Ray start, Ray end, bool _)
		{
			actionCallback(GetTerrainBlocks(start, end), start);
		}, showNoneCallback);
	}

	public bool PickBlockObjectArea(PlaceableBlockObjectSpec blockObjectSpec, Orientation orientation, FlipMode flipMode, BlockObjectAreaCallback previewCallback, BlockObjectAreaCallback actionCallback)
	{
		return _areaSelectionController.ProcessInput(delegate(Ray start, Ray end, bool _)
		{
			previewCallback(GetBlocks(start, end, blockObjectSpec, orientation, flipMode));
		}, delegate(Ray start, Ray end, bool _)
		{
			actionCallback(GetBlocks(start, end, blockObjectSpec, orientation, flipMode));
		}, delegate
		{
			previewCallback(Enumerable.Empty<Placement>());
		});
	}

	public void Reset()
	{
		_areaSelectionController.Reset();
		_segmentedLineDirection = LineDirection.SinglePoint;
	}

	private IEnumerable<Vector3Int> GetTerrainBlocks(Ray startRay, Ray endRay)
	{
		TraversedCoordinates? traversedCoordinates = _terrainPicker.PickTerrainCoordinates(startRay);
		if (!traversedCoordinates.HasValue)
		{
			return Enumerable.Empty<Vector3Int>();
		}
		Vector3Int coordinates = traversedCoordinates.Value.Coordinates;
		Vector3Int end = _terrainPicker.FindCoordinatesOnLevelInMap(endRay, coordinates.z + 1)?.Coordinates ?? coordinates;
		return _areaIterator.GetRectangle(coordinates, end, _maxBlocks);
	}

	private IEnumerable<Placement> GetBlocks(Ray startRay, Ray endRay, PlaceableBlockObjectSpec blockObjectSpec, Orientation orientation, FlipMode flipMode)
	{
		PickedCoordinates? pickedCoordinates = _blockObjectPreviewPicker.CenteredPreviewCoordinates(blockObjectSpec, orientation, startRay);
		if (pickedCoordinates.HasValue)
		{
			Vector3Int endCoords = GetEndCoords(startRay, endRay, pickedCoordinates.Value);
			return GetBlocksForLayout(blockObjectSpec, orientation, flipMode, pickedCoordinates.Value, endCoords);
		}
		return Enumerable.Empty<Placement>();
	}

	private Vector3Int GetEndCoords(Ray startRay, Ray endRay, PickedCoordinates startPlacement)
	{
		if (!startRay.Equals(endRay))
		{
			TraversedCoordinates? traversedCoordinates = _terrainPicker.FindCoordinatesOnLevelInMap(endRay, startPlacement.ReferenceTerrainLevel);
			if (traversedCoordinates.HasValue)
			{
				return traversedCoordinates.GetValueOrDefault().Coordinates + new Vector3Int(0, 0, startPlacement.VerticalOffset);
			}
		}
		return startPlacement.Coordinates;
	}

	private IEnumerable<Placement> GetBlocksForLayout(PlaceableBlockObjectSpec blockObjectSpec, Orientation orientation, FlipMode flipMode, PickedCoordinates pickedStartCoordinates, Vector3Int endCoords)
	{
		Vector3Int coordinates = pickedStartCoordinates.Coordinates;
		IEnumerable<Placement> placements = GetPlacements(blockObjectSpec, orientation, flipMode, coordinates, endCoords);
		return FilterPlacements(placements, pickedStartCoordinates.FilterOverhangingCoordinates);
	}

	private IEnumerable<Placement> GetPlacements(PlaceableBlockObjectSpec blockObjectSpec, Orientation orientation, FlipMode flipMode, Vector3Int startCoords, Vector3Int endCoords)
	{
		BlockObjectLayout layout = blockObjectSpec.Layout;
		int previewCount = layout.GetPreviewCount();
		return layout switch
		{
			BlockObjectLayout.Single => Enumerables.One(new Placement(startCoords, orientation, flipMode)), 
			BlockObjectLayout.Rectangle => RectangleCoordinates(previewCount, orientation, flipMode, startCoords, endCoords), 
			BlockObjectLayout.Line => LineCoordinates(previewCount, orientation, flipMode, startCoords, endCoords), 
			BlockObjectLayout.Half => HalvesCoordinates(blockObjectSpec, startCoords, orientation, flipMode), 
			BlockObjectLayout.SideLine => SideLineCoordinates(previewCount, orientation, flipMode, startCoords, endCoords), 
			BlockObjectLayout.TwoSegmentLine => TwoSegmentLineCoordinates(previewCount, orientation, flipMode, startCoords, endCoords), 
			_ => throw new ArgumentOutOfRangeException(string.Format("Unknown {0}: {1}", "BlockObjectLayout", layout)), 
		};
	}

	private IEnumerable<Placement> FilterPlacements(IEnumerable<Placement> placements, bool filterOverhangingCoordinates)
	{
		if (!filterOverhangingCoordinates)
		{
			return placements;
		}
		return placements.Where(PlacementHasStackableBelow);
	}

	private bool PlacementHasStackableBelow(Placement placement)
	{
		Vector3Int coords = placement.Coordinates.Below();
		if (!_terrainService.Underground(coords))
		{
			return _stackableBlockService.IsStackableBlockAt(coords);
		}
		return true;
	}

	private IEnumerable<Placement> RectangleCoordinates(int maxPoints, Orientation orientation, FlipMode flipMode, Vector3Int startCoords, Vector3Int endCoords)
	{
		return from coordinates in _areaIterator.GetRectangle(startCoords, endCoords, maxPoints)
			select new Placement(coordinates, orientation, flipMode);
	}

	private IEnumerable<Placement> LineCoordinates(int maxPoints, Orientation orientation, FlipMode flipMode, Vector3Int startCoords, Vector3Int endCoords)
	{
		IEnumerable<Vector3Int> line = _areaIterator.GetLine(startCoords, endCoords, maxPoints, out var direction);
		Orientation lineOrientation = ConvertOrientation(orientation, direction);
		return line.Select((Vector3Int coordinates) => new Placement(coordinates, lineOrientation, flipMode));
	}

	private static IEnumerable<Placement> HalvesCoordinates(PlaceableBlockObjectSpec blockObjectSpec, Vector3Int startCoords, Orientation orientation, FlipMode flipMode)
	{
		yield return new Placement(startCoords, orientation, flipMode);
		Vector3Int size = blockObjectSpec.GetSpec<BlockObjectSpec>().Size;
		int x = size.x - 1;
		int y = size.y * 2 - 1;
		Vector3Int coordinates = startCoords + orientation.Transform(new Vector3Int(x, y, 0));
		yield return new Placement(coordinates, orientation.Flip(), flipMode);
	}

	private IEnumerable<Placement> SideLineCoordinates(int maxPoints, Orientation orientation, FlipMode flipMode, Vector3Int startCoords, Vector3Int endCoords)
	{
		IEnumerable<Vector3Int> line = _areaIterator.GetLine(startCoords, endCoords, maxPoints, out var direction);
		Orientation orientation2 = ConvertOrientation(orientation, direction);
		if (orientation2.RotateClockwise() == orientation || orientation2.RotateCounterclockwise() == orientation)
		{
			return line.Select((Vector3Int coordinates) => new Placement(coordinates, orientation, flipMode));
		}
		return Enumerables.One(new Placement(startCoords, orientation, flipMode));
	}

	private IEnumerable<Placement> TwoSegmentLineCoordinates(int maxPoints, Orientation orientation, FlipMode flipMode, Vector3Int startCoords, Vector3Int endCoords)
	{
		IEnumerable<Vector3Int> enumerable = FirstSegmentLineCoordinates(maxPoints, startCoords, endCoords, out var lineDirection);
		_segmentedLineDirection = lineDirection;
		Orientation lineOrientation = ConvertOrientation(orientation, lineDirection);
		Vector3Int startCoords2 = Vector3Int.zero;
		foreach (Vector3Int coordinates in enumerable)
		{
			yield return new Placement(coordinates, lineOrientation, flipMode);
			startCoords2 = coordinates;
			maxPoints--;
		}
		IEnumerable<Placement> enumerable2 = SecondSegmentLineCoordinates(maxPoints, orientation, flipMode, startCoords2, endCoords);
		foreach (Placement item in enumerable2)
		{
			yield return item;
		}
	}

	private IEnumerable<Vector3Int> FirstSegmentLineCoordinates(int maxPoints, Vector3Int startCoords, Vector3Int endCoords, out LineDirection lineDirection)
	{
		if (_segmentedLineDirection == LineDirection.SinglePoint)
		{
			return _areaIterator.GetLine(startCoords, endCoords, maxPoints, out lineDirection);
		}
		return _areaIterator.GetLine(startCoords, endCoords, _segmentedLineDirection, maxPoints, out lineDirection);
	}

	private IEnumerable<Placement> SecondSegmentLineCoordinates(int pointsLeft, Orientation orientation, FlipMode flipMode, Vector3Int startCoords, Vector3Int endCoords)
	{
		if (pointsLeft > 0)
		{
			IEnumerable<Vector3Int> source = _areaIterator.GetLine(startCoords, endCoords, pointsLeft + 1, out var direction).Skip(1);
			Orientation lineOrientation = ConvertOrientation(orientation, direction);
			return source.Select((Vector3Int coordinates) => new Placement(coordinates, lineOrientation, flipMode));
		}
		return Enumerable.Empty<Placement>();
	}

	private static Orientation ConvertOrientation(Orientation blockObjectOrientation, LineDirection lineDirection)
	{
		return lineDirection switch
		{
			LineDirection.SinglePoint => blockObjectOrientation, 
			LineDirection.Down => Orientation.Cw0, 
			LineDirection.Left => Orientation.Cw90, 
			LineDirection.Up => Orientation.Cw180, 
			LineDirection.Right => Orientation.Cw270, 
			_ => throw new ArgumentOutOfRangeException("lineDirection", lineDirection, null), 
		};
	}
}
