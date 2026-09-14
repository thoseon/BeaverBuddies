using System;
using Timberborn.Common;
using Timberborn.Multithreading;
using Timberborn.TerrainSystem;
using Timberborn.WaterSystem;
using UnityEngine;

namespace Timberborn.SoilMoistureSystem;

internal readonly struct MoistureCalculationTask(WaterColumnRetriever waterColumnRetriever, CeilingRetriever ceilingRetriever, float[] moistureLevels, bool[] moistureLevelsChangedLastTick, in ReadOnlyArray<byte> waterColumnCounts, in ReadOnlyArray<ReadOnlyWaterColumn> waterColumns, in ReadOnlyArray<byte> terrainColumnCounts, in ReadOnlyArray<ReadOnlyTerrainColumn> terrainColumns, in ReadOnlyArray<float> lastTickMoistureLevels, in ReadOnlyArray<bool> fullMoistureBarriers, in ReadOnlyArray<bool> aboveMoistureBarriers, in ReadOnlyArray<byte> clusterSaturation, int xMapSize, int stride, int verticalStride, float moistureDecayRate, float moistureSpreadingRate, float minimumWaterContamination, int verticalSpreadCostMultiplier, float waterContaminationScaler) : IParallelizerLoopTask
{
	private static readonly float MinimumMoisture = 0.01f;

	private readonly WaterColumnRetriever _waterColumnRetriever = waterColumnRetriever;

	private readonly CeilingRetriever _ceilingRetriever = ceilingRetriever;

	private readonly float[] _moistureLevels = moistureLevels;

	private readonly bool[] _moistureLevelsChangedLastTick = moistureLevelsChangedLastTick;

	private readonly ReadOnlyArray<byte> _waterColumnCounts = waterColumnCounts;

	private readonly ReadOnlyArray<ReadOnlyWaterColumn> _waterColumns = waterColumns;

	private readonly ReadOnlyArray<byte> _terrainColumnCounts = terrainColumnCounts;

	private readonly ReadOnlyArray<ReadOnlyTerrainColumn> _terrainColumns = terrainColumns;

	private readonly ReadOnlyArray<float> _lastTickMoistureLevels = lastTickMoistureLevels;

	private readonly ReadOnlyArray<bool> _fullMoistureBarriers = fullMoistureBarriers;

	private readonly ReadOnlyArray<bool> _aboveMoistureBarriers = aboveMoistureBarriers;

	private readonly ReadOnlyArray<byte> _clusterSaturation = clusterSaturation;

	private readonly int _xMapSize = xMapSize;

	private readonly int _stride = stride;

	private readonly int _verticalStride = verticalStride;

	private readonly float _moistureDecayRate = moistureDecayRate;

	private readonly float _moistureSpreadingRate = moistureSpreadingRate;

	private readonly float _minimumWaterContamination = minimumWaterContamination;

	private readonly int _verticalSpreadCostMultiplier = verticalSpreadCostMultiplier;

	private readonly float _waterContaminationScaler = waterContaminationScaler;

	public void Run(int y)
	{
		int num = (y + 1) * _stride;
		for (int i = 0; i < _xMapSize; i++)
		{
			int num2 = i + 1 + num;
			byte b = _terrainColumnCounts[num2];
			for (int j = 0; j < b; j++)
			{
				int num3 = j * _verticalStride + num2;
				float num4 = CalculateMoistureForCell(num2, num3);
				_moistureLevels[num3] = num4;
				if (num4 != _lastTickMoistureLevels[num3])
				{
					_moistureLevelsChangedLastTick[num3] = true;
				}
			}
		}
	}

	private float CalculateMoistureForCell(int index2D, int index3D)
	{
		ref readonly ReadOnlyTerrainColumn reference = ref _terrainColumns[index3D];
		int ceiling = reference.Ceiling;
		int floor = reference.Floor;
		int index = ceiling * _verticalStride + index2D;
		if (_fullMoistureBarriers[index])
		{
			return 0f;
		}
		bool flag = _waterColumnRetriever.TryGetColumnWithFloorAtHeight(in _waterColumnCounts, in _waterColumns, _verticalStride, index2D, ceiling, out var index3D2);
		bool flag2 = _aboveMoistureBarriers[index];
		if (flag)
		{
			ref readonly ReadOnlyWaterColumn reference2 = ref _waterColumns[index3D2];
			if (reference2.WaterDepth > 0f && reference2.Contamination <= _minimumWaterContamination && !flag2)
			{
				return (int)GetInitialMoisturizerRange(index3D2);
			}
		}
		float num = 0f;
		if (_waterColumnRetriever.TryGetColumnWithCeilingAtHeight(in _waterColumnCounts, in _waterColumns, _verticalStride, index2D, floor, out var index3D3))
		{
			ref readonly ReadOnlyWaterColumn reference3 = ref _waterColumns[index3D3];
			if (reference3.WaterDepth + (float)(int)reference3.Floor >= (float)(int)reference3.Ceiling)
			{
				num = GetMoisture(index3D3, ceiling - floor - 1);
			}
		}
		int num2 = index2D - _stride;
		int num3 = index2D - 1;
		int num4 = index2D + 1;
		int num5 = index2D + _stride;
		if (num < 16f)
		{
			int moistureFromWater = GetMoistureFromWater(num2, ceiling, floor, flag2);
			if ((float)moistureFromWater > num)
			{
				num = moistureFromWater;
			}
		}
		if (num < 16f)
		{
			int moistureFromWater2 = GetMoistureFromWater(num3, ceiling, floor, flag2);
			if ((float)moistureFromWater2 > num)
			{
				num = moistureFromWater2;
			}
		}
		if (num < 16f)
		{
			int moistureFromWater3 = GetMoistureFromWater(num5, ceiling, floor, flag2);
			if ((float)moistureFromWater3 > num)
			{
				num = moistureFromWater3;
			}
		}
		if (num < 16f)
		{
			int moistureFromWater4 = GetMoistureFromWater(num4, ceiling, floor, flag2);
			if ((float)moistureFromWater4 > num)
			{
				num = moistureFromWater4;
			}
		}
		float num6 = 0f;
		if (num < 16f)
		{
			int neighborIndex2D = index2D - _stride - 1;
			int neighborIndex2D2 = index2D - _stride + 1;
			int neighborIndex2D3 = index2D + _stride - 1;
			int neighborIndex2D4 = index2D + _stride + 1;
			num6 = GetMoistureFromNeighbor(num2, floor, ceiling, num6, 1f);
			num6 = GetMoistureFromNeighbor(num3, floor, ceiling, num6, 1f);
			num6 = GetMoistureFromNeighbor(num4, floor, ceiling, num6, 1f);
			num6 = GetMoistureFromNeighbor(num5, floor, ceiling, num6, 1f);
			num6 = GetMoistureFromNeighbor(neighborIndex2D, floor, ceiling, num6, 1.414f);
			num6 = GetMoistureFromNeighbor(neighborIndex2D2, floor, ceiling, num6, 1.414f);
			num6 = GetMoistureFromNeighbor(neighborIndex2D3, floor, ceiling, num6, 1.414f);
			num6 = GetMoistureFromNeighbor(neighborIndex2D4, floor, ceiling, num6, 1.414f);
		}
		float num7 = _lastTickMoistureLevels[index3D];
		float num8 = num7 - _moistureDecayRate;
		if (num8 < 0f)
		{
			num8 = 0f;
		}
		float num9 = num8;
		if (num > num8 && num >= num6)
		{
			float num10 = num7 + _moistureSpreadingRate;
			num9 = ((num > num10) ? num10 : num);
		}
		else if (num6 > num8)
		{
			num9 = num6;
		}
		float num11 = (flag ? _waterColumns[index3D2].Contamination : 0f);
		float num12 = num9 * (1f - num11);
		if (num12 < MinimumMoisture)
		{
			return 0f;
		}
		return num12;
	}

	private int GetMoistureFromWater(int neighborIndex, int originTerrainHeight, int originTerrainFloor, bool aboveBarrier)
	{
		if (!_waterColumnRetriever.TryGetTopWateredColumn(in _waterColumnCounts, in _waterColumns, _verticalStride, originTerrainHeight, neighborIndex, out var index3D))
		{
			return 0;
		}
		ref readonly ReadOnlyWaterColumn reference = ref _waterColumns[index3D];
		float waterDepth = reference.WaterDepth;
		byte floor = reference.Floor;
		int num = ((waterDepth > 0f) ? Mathf.CeilToInt((float)(int)floor + waterDepth) : 0);
		if (num <= originTerrainFloor)
		{
			return 0;
		}
		if (floor >= originTerrainHeight)
		{
			if (aboveBarrier)
			{
				return 0;
			}
			int ceilingAtOrBelowHeight = _ceilingRetriever.GetCeilingAtOrBelowHeight(in _terrainColumnCounts, in _terrainColumns, _verticalStride, neighborIndex, floor);
			int index = neighborIndex + _verticalStride * ceilingAtOrBelowHeight;
			if (_aboveMoistureBarriers[index])
			{
				return 0;
			}
		}
		int num2 = originTerrainHeight - num;
		if (num2 < 0)
		{
			num2 = 0;
		}
		return GetMoisture(index3D, num2);
	}

	private int GetMoisture(int wateredNeighborIndex3D, int heightDifference)
	{
		return GetMoisture(wateredNeighborIndex3D) - heightDifference * _verticalSpreadCostMultiplier;
	}

	private int GetMoisture(int index3D)
	{
		float contamination = _waterColumns[index3D].Contamination;
		if (contamination < _minimumWaterContamination)
		{
			return (int)GetInitialMoisturizerRange(index3D);
		}
		float num = contamination * _waterContaminationScaler;
		if (num >= 1f)
		{
			return 0;
		}
		return (int)(GetInitialMoisturizerRange(index3D) * (1f - num));
	}

	private float GetInitialMoisturizerRange(int index3D)
	{
		return 2f * (float)(int)_clusterSaturation[index3D];
	}

	private float GetMoistureFromNeighbor(int neighborIndex2D, int floor, int height, float currentMoisture, float cost)
	{
		byte b = _terrainColumnCounts[neighborIndex2D];
		for (int i = 0; i < b; i++)
		{
			int num = i * _verticalStride + neighborIndex2D;
			ref readonly ReadOnlyTerrainColumn reference = ref _terrainColumns[num];
			int ceiling = reference.Ceiling;
			if (ceiling >= floor)
			{
				if (reference.Floor > height)
				{
					break;
				}
				float moistureFromNeighbor = GetMoistureFromNeighbor(neighborIndex2D, num, ceiling, height, cost);
				if (moistureFromNeighbor > currentMoisture)
				{
					currentMoisture = moistureFromNeighbor;
				}
			}
		}
		return currentMoisture;
	}

	private float GetMoistureFromNeighbor(int neighborIndex2D, int neighborColumnIndex3D, int neighborTerrainHeight, int originTerrainHeight, float spreadCost)
	{
		float num = _lastTickMoistureLevels[neighborColumnIndex3D];
		if (num == 0f)
		{
			return 0f;
		}
		int num2 = originTerrainHeight - neighborTerrainHeight;
		if (num2 < 0)
		{
			return num - spreadCost;
		}
		int num3 = (int)Math.Ceiling(_waterColumnRetriever.GetColumn(_waterColumnCounts, _waterColumns, _verticalStride, neighborIndex2D, neighborTerrainHeight).WaterDepth);
		int num4 = num2 - num3;
		if (num4 < 0)
		{
			return num - spreadCost;
		}
		return num - (float)(num4 * _verticalSpreadCostMultiplier) - spreadCost;
	}
}
