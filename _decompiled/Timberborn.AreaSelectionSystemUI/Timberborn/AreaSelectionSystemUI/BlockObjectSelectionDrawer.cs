using System.Collections.Generic;
using Timberborn.AreaSelectionSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.SelectionSystem;
using UnityEngine;

namespace Timberborn.AreaSelectionSystemUI;

public class BlockObjectSelectionDrawer
{
	private readonly RectangleBoundsDrawer _rectangleBoundsDrawer;

	private readonly RollingHighlighter _rollingHighlighter;

	private readonly Color _blockObjectHighlightColor;

	private readonly MeasurableAreaDrawer _measurableAreaDrawer;

	private Vector3Int _start;

	private Vector3Int _end;

	private bool _selectingArea;

	public BlockObjectSelectionDrawer(RectangleBoundsDrawer rectangleBoundsDrawer, RollingHighlighter rollingHighlighter, Color blockObjectHighlightColor, MeasurableAreaDrawer measurableAreaDrawer)
	{
		_rectangleBoundsDrawer = rectangleBoundsDrawer;
		_rollingHighlighter = rollingHighlighter;
		_blockObjectHighlightColor = blockObjectHighlightColor;
		_measurableAreaDrawer = measurableAreaDrawer;
	}

	public void Draw(IEnumerable<BlockObject> blockObjects, Vector3Int start, Vector3Int end, bool selectingArea)
	{
		_start = start;
		_end = end;
		_selectingArea = selectingArea;
		Draw();
		_rollingHighlighter.HighlightPrimary(blockObjects, _blockObjectHighlightColor);
	}

	public void StopDrawing()
	{
		_rollingHighlighter.UnhighlightAllPrimary();
	}

	private void Draw()
	{
		if (_selectingArea)
		{
			_rectangleBoundsDrawer.DrawOnLevel(_start.XY(), _end.XY(), _start.z);
			DrawAreaMeasurement();
		}
	}

	private void DrawAreaMeasurement()
	{
		(Vector2Int min, Vector2Int max) tuple = Vectors.MinMax(_start.XY(), _end.XY());
		Vector2Int item = tuple.min;
		Vector2Int item2 = tuple.max;
		for (int i = item.x; i <= item2.x; i++)
		{
			for (int j = item.y; j <= item2.y; j++)
			{
				_measurableAreaDrawer.AddMeasurableCoordinates(new Vector3Int(i, j, 0));
			}
		}
	}
}
