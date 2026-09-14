using System.Text;
using Timberborn.Common;
using Timberborn.CursorToolSystem;
using Timberborn.DebuggingUI;
using Timberborn.MapIndexSystem;
using Timberborn.SingletonSystem;
using Timberborn.SoilContaminationSystem;
using Timberborn.SoilMoistureSystem;
using Timberborn.TerrainSystem;
using Timberborn.TerrainSystemRendering;
using UnityEngine;

namespace Timberborn.TerrainSystemUI;

internal class TerrainDebuggingPanel : ILoadableSingleton, IDebuggingPanel
{
	private readonly DebuggingPanel _debuggingPanel;

	private readonly CursorDebugger _cursorDebugger;

	private readonly IThreadSafeColumnTerrainMap _threadSafeColumnTerrainMap;

	private readonly MapIndexService _mapIndexService;

	private readonly ISoilMoistureService _soilMoistureService;

	private readonly ISoilContaminationService _soilContaminationService;

	private readonly TerrainMaterialMap _terrainMaterialMap;

	private readonly StringBuilder _text = new StringBuilder();

	public TerrainDebuggingPanel(DebuggingPanel debuggingPanel, CursorDebugger cursorDebugger, IThreadSafeColumnTerrainMap threadSafeColumnTerrainMap, MapIndexService mapIndexService, ISoilMoistureService soilMoistureService, ISoilContaminationService soilContaminationService, TerrainMaterialMap terrainMaterialMap)
	{
		_debuggingPanel = debuggingPanel;
		_cursorDebugger = cursorDebugger;
		_threadSafeColumnTerrainMap = threadSafeColumnTerrainMap;
		_mapIndexService = mapIndexService;
		_soilMoistureService = soilMoistureService;
		_soilContaminationService = soilContaminationService;
		_terrainMaterialMap = terrainMaterialMap;
	}

	public void Load()
	{
		_debuggingPanel.AddDebuggingPanel(this, "Terrain columns");
	}

	public string GetText()
	{
		if (_cursorDebugger.Active)
		{
			Vector2Int vector2Int = _cursorDebugger.Coordinates.XY();
			int num = _mapIndexService.CellToIndex(vector2Int);
			int columnCount = _threadSafeColumnTerrainMap.GetColumnCount(num);
			for (int i = 0; i < columnCount; i++)
			{
				int num2 = num + i * _mapIndexService.VerticalStride;
				int columnFloor = _threadSafeColumnTerrainMap.GetColumnFloor(num2);
				int columnCeiling = _threadSafeColumnTerrainMap.GetColumnCeiling(num2);
				_text.AppendLine($"Column {columnFloor} - {columnCeiling}");
				_text.AppendLine($"  - Soil moisture: {_soilMoistureService.SoilMoisture(num2):0.00}");
				float desertIntensity = _terrainMaterialMap.GetDesertIntensity(vector2Int.ToVector3Int(columnCeiling));
				_text.AppendLine($"  - Desert intensity: {desertIntensity:0.00}");
				_text.AppendLine($"  - Soil contamination: {_soilContaminationService.Contamination(num2):0.00}");
			}
			return _text.ToStringAndClear();
		}
		return null;
	}
}
