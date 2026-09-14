using System.Text;
using Timberborn.Common;
using Timberborn.CursorToolSystem;
using Timberborn.Debugging;
using Timberborn.DebuggingUI;
using Timberborn.MapIndexSystem;
using Timberborn.SingletonSystem;
using Timberborn.TickSystem;
using Timberborn.WaterSystem;
using UnityEngine;

namespace Timberborn.WaterSystemUI;

internal class WaterColumnDebuggingPanel : ILoadableSingleton, ITickableSingleton, IDebuggingPanel
{
	private readonly DebuggingPanel _debuggingPanel;

	private readonly CursorDebugger _cursorDebugger;

	private readonly INonThreadSafeWaterService _nonThreadSafeWaterService;

	private readonly MapIndexService _mapIndexService;

	private readonly DebugModeManager _debugModeManager;

	private readonly EventBus _eventBus;

	private readonly StringBuilder _text = new StringBuilder();

	private bool _dataUpdated;

	public WaterColumnDebuggingPanel(DebuggingPanel debuggingPanel, CursorDebugger cursorDebugger, INonThreadSafeWaterService nonThreadSafeWaterService, MapIndexService mapIndexService, DebugModeManager debugModeManager, EventBus eventBus)
	{
		_debuggingPanel = debuggingPanel;
		_cursorDebugger = cursorDebugger;
		_nonThreadSafeWaterService = nonThreadSafeWaterService;
		_mapIndexService = mapIndexService;
		_debugModeManager = debugModeManager;
		_eventBus = eventBus;
	}

	public void Load()
	{
		_debuggingPanel.AddDebuggingPanel(this, "Water columns");
		_eventBus.Register(this);
	}

	public void Tick()
	{
		if (_debugModeManager.Enabled)
		{
			_dataUpdated = true;
			_nonThreadSafeWaterService.UpdateOutflowsData();
		}
	}

	[OnEvent]
	public void OnDebugModeToggled(DebugModeToggledEvent debugModeToggledEvent)
	{
		_dataUpdated = false;
	}

	public string GetText()
	{
		if (_cursorDebugger.Active)
		{
			_text.Clear();
			Vector2Int coordinates = _cursorDebugger.Coordinates.XY();
			int num = _mapIndexService.CellToIndex(coordinates);
			_text.AppendLine($"Coords: {coordinates.x}, {coordinates.y} (index: {num})");
			for (int i = 0; i < _nonThreadSafeWaterService.GetColumnCount(num); i++)
			{
				int index3D = num + i * _mapIndexService.VerticalStride;
				ReadOnlyWaterColumn columnByIndex = _nonThreadSafeWaterService.GetColumnByIndex(index3D);
				_text.AppendLine($"Column {columnByIndex.Floor} - {columnByIndex.Ceiling}:");
				_text.AppendLine($" - Water depth: {columnByIndex.WaterDepth}");
				_text.AppendLine($" - Water height: {(float)(int)columnByIndex.Floor + columnByIndex.WaterDepth}");
				_text.AppendLine($" - Contamination: {columnByIndex.Contamination:F5}");
				_text.AppendLine($" - Overflow: {columnByIndex.Overflow}");
				_text.AppendLine(" - Outflows:");
				AppendOutflows(index3D);
			}
		}
		return _text.ToStringWithoutNewLineEnd();
	}

	private void AppendOutflows(int index3D)
	{
		if (!_dataUpdated)
		{
			_text.AppendLine("   - Waiting for tick...");
			return;
		}
		ReadOnlyColumnOutflows outflows = _nonThreadSafeWaterService.ColumnOutflows(index3D);
		if (outflows.Outflows != null)
		{
			AppendAllOutflows(outflows);
			return;
		}
		string text = "B=" + FormatOutflow(outflows.BottomFlow) + ", L=" + FormatOutflow(outflows.LeftFlow) + ", T=" + FormatOutflow(outflows.TopFlow) + ", R=" + FormatOutflow(outflows.RightFlow);
		_text.AppendLine("  - " + text);
	}

	private void AppendAllOutflows(ReadOnlyColumnOutflows outflows)
	{
		if (outflows.BottomFlow.Flow > 0f)
		{
			AppendFormattedOutflow(outflows.BottomFlow);
		}
		if (outflows.LeftFlow.Flow > 0f)
		{
			AppendFormattedOutflow(outflows.LeftFlow);
		}
		if (outflows.TopFlow.Flow > 0f)
		{
			AppendFormattedOutflow(outflows.TopFlow);
		}
		if (outflows.RightFlow.Flow > 0f)
		{
			AppendFormattedOutflow(outflows.RightFlow);
		}
		foreach (TargetedFlow outflow in outflows.Outflows)
		{
			AppendFormattedOutflow(outflow);
		}
	}

	private void AppendFormattedOutflow(TargetedFlow outflow)
	{
		ReadOnlyWaterColumn columnByIndex = _nonThreadSafeWaterService.GetColumnByIndex(outflow.Index3D);
		_text.AppendLine($"  - {columnByIndex.Floor}: {FormatOutflow(outflow)}");
	}

	private static string FormatOutflow(TargetedFlow outflow)
	{
		return $"{outflow.Flow:0.000}";
	}
}
