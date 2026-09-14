using System.Collections.Generic;
using Timberborn.Common;
using Timberborn.MapIndexSystem;
using Timberborn.Multithreading;
using UnityEngine;

namespace Timberborn.WaterSystem;

internal readonly struct UpdateWaterChangesTask(MapIndexService mapIndexService, WaterDepthSetter waterDepthSetter, MutableWaterColumnRetriever mutableWaterColumnRetriever, Dictionary<Vector3Int, WaterAmountChange> removedWater, WaterColumn[] waterColumns, ReadOnlyArray<byte> waterColumnCounts, ReadOnlyList<WaterChange> waterChanges, int verticalStride, float overflowPressureFactor, float maxWaterContamination) : IParallelizerSingleTask
{
	private readonly MapIndexService _mapIndexService = mapIndexService;

	private readonly WaterDepthSetter _waterDepthSetter = waterDepthSetter;

	private readonly MutableWaterColumnRetriever _mutableWaterColumnRetriever = mutableWaterColumnRetriever;

	private readonly Dictionary<Vector3Int, WaterAmountChange> _removedWater = removedWater;

	private readonly WaterColumn[] _waterColumns = waterColumns;

	private readonly ReadOnlyArray<byte> _waterColumnCounts = waterColumnCounts;

	private readonly ReadOnlyList<WaterChange> _waterChanges = waterChanges;

	private readonly int _verticalStride = verticalStride;

	private readonly float _overflowPressureFactor = overflowPressureFactor;

	private readonly float _maxWaterContamination = maxWaterContamination;

	public void Run()
	{
		_removedWater.Clear();
		for (int i = 0; i < _waterChanges.Count; i++)
		{
			WaterChange waterChange = _waterChanges[i];
			Vector3Int coordinates = waterChange.Coordinates;
			int index = _mapIndexService.CellToIndex(coordinates.XY());
			if (!_mutableWaterColumnRetriever.TryGetColumnIndex(_waterColumnCounts.AsSpan, _waterColumns, _verticalStride, index, coordinates.z, out var columnIndex))
			{
				continue;
			}
			ref WaterColumn reference = ref _waterColumns[columnIndex];
			float num = reference.WaterDepth + reference.Overflow * _overflowPressureFactor;
			float contamination = reference.Contamination;
			float num2 = num * (1f - contamination);
			float num3 = num * contamination;
			float num4 = reference.WaterDepth + reference.Overflow;
			float depthChange = waterChange.DepthChange;
			float contaminationChange = waterChange.ContaminationChange;
			float num5 = num2 + depthChange * (1f - contaminationChange);
			num5 = ((num5 < 0f) ? 0f : num5);
			float num6 = num3 + depthChange * contaminationChange;
			num6 = ((num6 < 0f) ? 0f : num6);
			float num7 = num6 + num5;
			_waterDepthSetter.SetWaterDepth(num7 - num, ref reference);
			UpdateContamination(ref reference, num6, num7);
			if (waterChange.DepthChange < 0f)
			{
				float num8 = reference.WaterDepth + reference.Overflow;
				float num9 = num4 * (1f - contamination) - num8 * (1f - reference.Contamination);
				float num10 = num4 * contamination - num8 * reference.Contamination;
				if (_removedWater.TryGetValue(coordinates, out var value))
				{
					float cleanWaterChange = num9 + value.CleanWaterChange;
					float contaminatedWaterChange = num10 + value.ContaminatedWaterChange;
					_removedWater[coordinates] = new WaterAmountChange(cleanWaterChange, contaminatedWaterChange);
				}
				else
				{
					_removedWater[coordinates] = new WaterAmountChange(num9, num10);
				}
			}
		}
	}

	private void UpdateContamination(ref WaterColumn waterColumn, float contaminatedWater, float totalWater)
	{
		if (waterColumn.WaterDepth + waterColumn.Overflow * _overflowPressureFactor == 0f || totalWater <= 0f)
		{
			waterColumn.Contamination = 0f;
			return;
		}
		float num = contaminatedWater / totalWater;
		waterColumn.Contamination = ((num > _maxWaterContamination) ? _maxWaterContamination : num);
	}
}
