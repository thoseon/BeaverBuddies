using Timberborn.Common;
using Timberborn.DebuggingUI;
using Timberborn.MapIndexSystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.CursorToolSystem;

internal class CursorDebuggingPanel : ILoadableSingleton, IDebuggingPanel
{
	private readonly DebuggingPanel _debuggingPanel;

	private readonly CursorDebugger _cursorDebugger;

	private readonly MapIndexService _mapIndexService;

	public CursorDebuggingPanel(DebuggingPanel debuggingPanel, CursorDebugger cursorDebugger, MapIndexService mapIndexService)
	{
		_debuggingPanel = debuggingPanel;
		_cursorDebugger = cursorDebugger;
		_mapIndexService = mapIndexService;
	}

	public void Load()
	{
		_debuggingPanel.AddDebuggingPanel(this, "Cursor");
	}

	public string GetText()
	{
		if (_cursorDebugger.Active)
		{
			Vector3Int coordinates = _cursorDebugger.Coordinates;
			Vector3 position = _cursorDebugger.Position;
			return $"Block coordinates: {coordinates}" + $"\nIntersection position: {position}" + $"\nMap index: {_mapIndexService.CellToIndex(coordinates.XY())}";
		}
		return null;
	}
}
