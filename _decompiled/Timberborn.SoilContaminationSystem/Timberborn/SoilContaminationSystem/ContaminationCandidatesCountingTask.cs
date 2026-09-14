using Timberborn.Common;
using Timberborn.Multithreading;
using Timberborn.TerrainSystem;
using Timberborn.WaterSystem;
using UnityEngine;

namespace Timberborn.SoilContaminationSystem;

internal readonly struct ContaminationCandidatesCountingTask : IParallelizerLoopTask
{
	private readonly WaterColumnRetriever _waterColumnRetriever;

	private readonly CeilingRetriever _ceilingRetriever;

	private readonly float[] _contaminationCandidates;

	private readonly ReadOnlyArray<byte> _waterColumnCounts;

	private readonly ReadOnlyArray<ReadOnlyWaterColumn> _waterColumns;

	private readonly ReadOnlyArray<byte> _terrainColumnCounts;

	private readonly ReadOnlyArray<ReadOnlyTerrainColumn> _terrainColumns;

	private readonly ReadOnlyArray<bool> _contaminationBarriers;

	private readonly ReadOnlyArray<bool> _aboveMoistureBarriers;

	private readonly ReadOnlyArray<float> _lastTickContaminationCandidates;

	private readonly int _xMapSize;

	private readonly int _stride;

	private readonly int _verticalStride;

	private readonly float _maximumSoilContamination;

	private readonly float _regularSpreadCost;

	private readonly float _diagonalSpreadCost;

	private readonly float _contaminationDecayRate;

	private readonly float _contaminationSpreadingRate;

	private readonly float _minimumWaterContamination;

	private readonly float _contaminationScaler;

	private readonly float _verticalCostModifier;

	public ContaminationCandidatesCountingTask(WaterColumnRetriever waterColumnRetriever, CeilingRetriever ceilingRetriever, float[] contaminationCandidates, ReadOnlyArray<byte> waterColumnCounts, ReadOnlyArray<ReadOnlyWaterColumn> waterColumns, ReadOnlyArray<byte> terrainColumnCounts, ReadOnlyArray<ReadOnlyTerrainColumn> terrainColumns, ReadOnlyArray<bool> contaminationBarriers, ReadOnlyArray<bool> aboveMoistureBarriers, ReadOnlyArray<float> lastTickContaminationCandidates, int xMapSize, int stride, int verticalStride, float maximumSoilContamination, float regularSpreadCost, float diagonalSpreadCost, float contaminationDecayRate, float contaminationSpreadingRate, float minimumWaterContamination, float contaminationScaler, float verticalCostModifier)
	{
		this = default(ContaminationCandidatesCountingTask);
		_waterColumnRetriever = waterColumnRetriever;
		_ceilingRetriever = ceilingRetriever;
		_contaminationCandidates = contaminationCandidates;
		_waterColumnCounts = waterColumnCounts;
		_waterColumns = waterColumns;
		_terrainColumnCounts = terrainColumnCounts;
		_terrainColumns = terrainColumns;
		_contaminationBarriers = contaminationBarriers;
		_aboveMoistureBarriers = aboveMoistureBarriers;
		_lastTickContaminationCandidates = lastTickContaminationCandidates;
		_xMapSize = xMapSize;
		_stride = stride;
		_verticalStride = verticalStride;
		_maximumSoilContamination = maximumSoilContamination;
		_regularSpreadCost = regularSpreadCost;
		_diagonalSpreadCost = diagonalSpreadCost;
		_contaminationDecayRate = contaminationDecayRate;
		_contaminationSpreadingRate = contaminationSpreadingRate;
		_minimumWaterContamination = minimumWaterContamination;
		_contaminationScaler = contaminationScaler;
		_verticalCostModifier = verticalCostModifier;
	}

	public void Run(int y)
	{
		int num = (y + 1) * _stride;
		for (int i = 0; i < _xMapSize; i++)
		{
			int num2 = i + 1 + num;
			byte b = _terrainColumnCounts[num2];
			for (int j = 0; j < b; j++)
			{
				int num3 = num2 + j * _verticalStride;
				_contaminationCandidates[num3] = GetContaminationCandidate(num2, num3);
			}
		}
	}

	private float GetContaminationCandidate(int index2D, int index3D)
	{
		ref readonly ReadOnlyTerrainColumn reference = ref _terrainColumns[index3D];
		int ceiling = reference.Ceiling;
		int floor = reference.Floor;
		int index = ceiling * _verticalStride + index2D;
		if (_contaminationBarriers[index])
		{
			return 0f;
		}
		float num = 0f;
		if (_waterColumnRetriever.TryGetColumnWithCeilingAtHeight(in _waterColumnCounts, in _waterColumns, _verticalStride, index2D, floor, out var index3D2))
		{
			ref readonly ReadOnlyWaterColumn reference2 = ref _waterColumns[index3D2];
			if (reference2.WaterDepth + (float)(int)reference2.Floor >= (float)(int)reference2.Ceiling)
			{
				num = GetContaminationFromWaterBelow(index3D2, ceiling - floor - 1);
			}
		}
		int num2 = index2D - _stride;
		int num3 = index2D - 1;
		int num4 = index2D + 1;
		int num5 = index2D + _stride;
		bool aboveBarrier = _aboveMoistureBarriers[index];
		if (num < _maximumSoilContamination)
		{
			float contaminationFromWater = GetContaminationFromWater(num2, ceiling, floor, aboveBarrier);
			if (contaminationFromWater > num)
			{
				num = contaminationFromWater;
			}
		}
		if (num < _maximumSoilContamination)
		{
			float contaminationFromWater2 = GetContaminationFromWater(num3, ceiling, floor, aboveBarrier);
			if (contaminationFromWater2 > num)
			{
				num = contaminationFromWater2;
			}
		}
		if (num < _maximumSoilContamination)
		{
			float contaminationFromWater3 = GetContaminationFromWater(num5, ceiling, floor, aboveBarrier);
			if (contaminationFromWater3 > num)
			{
				num = contaminationFromWater3;
			}
		}
		if (num < _maximumSoilContamination)
		{
			float contaminationFromWater4 = GetContaminationFromWater(num4, ceiling, floor, aboveBarrier);
			if (contaminationFromWater4 > num)
			{
				num = contaminationFromWater4;
			}
		}
		float num6 = 0f;
		if (num < _maximumSoilContamination)
		{
			int neighborIndex2D = index2D - _stride - 1;
			int neighborIndex2D2 = index2D - _stride + 1;
			int neighborIndex2D3 = index2D + _stride - 1;
			int neighborIndex2D4 = index2D + _stride + 1;
			num6 = GetContaminationFromNeighbor(num2, floor, ceiling, num6, _regularSpreadCost);
			num6 = GetContaminationFromNeighbor(num3, floor, ceiling, num6, _regularSpreadCost);
			num6 = GetContaminationFromNeighbor(num4, floor, ceiling, num6, _regularSpreadCost);
			num6 = GetContaminationFromNeighbor(num5, floor, ceiling, num6, _regularSpreadCost);
			num6 = GetContaminationFromNeighbor(neighborIndex2D, floor, ceiling, num6, _diagonalSpreadCost);
			num6 = GetContaminationFromNeighbor(neighborIndex2D2, floor, ceiling, num6, _diagonalSpreadCost);
			num6 = GetContaminationFromNeighbor(neighborIndex2D3, floor, ceiling, num6, _diagonalSpreadCost);
			num6 = GetContaminationFromNeighbor(neighborIndex2D4, floor, ceiling, num6, _diagonalSpreadCost);
		}
		float num7 = _lastTickContaminationCandidates[index3D];
		float num8 = num7 - _contaminationDecayRate;
		if (num8 < 0f)
		{
			num8 = 0f;
		}
		if (num > num8 && num >= num6)
		{
			float num9 = num7 + _contaminationSpreadingRate;
			if (!(num > num9))
			{
				return num;
			}
			return num9;
		}
		if (num6 > num8)
		{
			return num6;
		}
		return num8;
	}

	private float GetContaminationFromWaterBelow(int waterIndex3D, int heightDifference)
	{
		float num = _waterColumns[waterIndex3D].Contamination - _minimumWaterContamination;
		if (num < 0f)
		{
			return 0f;
		}
		float num2 = num * _contaminationScaler;
		float num3 = (float)heightDifference * _verticalCostModifier;
		return num2 - num3;
	}

	private float GetContaminationFromWater(int neighborIndex, int originTerrainHeight, int originTerrainFloor, bool aboveBarrier)
	{
		if (!_waterColumnRetriever.TryGetTopContaminatedColumn(in _waterColumnCounts, in _waterColumns, _verticalStride, originTerrainHeight, neighborIndex, out var index3D))
		{
			return 0f;
		}
		ref readonly ReadOnlyWaterColumn reference = ref _waterColumns[index3D];
		float num = reference.Contamination - _minimumWaterContamination;
		if (num < 0f)
		{
			return 0f;
		}
		float waterDepth = reference.WaterDepth;
		byte floor = reference.Floor;
		int num2 = ((waterDepth > 0f) ? Mathf.CeilToInt((float)(int)floor + waterDepth) : 0);
		if (num2 <= originTerrainFloor)
		{
			return 0f;
		}
		if (floor >= originTerrainHeight)
		{
			if (aboveBarrier)
			{
				return 0f;
			}
			int ceilingAtOrBelowHeight = _ceilingRetriever.GetCeilingAtOrBelowHeight(in _terrainColumnCounts, in _terrainColumns, _verticalStride, neighborIndex, floor);
			int index = neighborIndex + _verticalStride * ceilingAtOrBelowHeight;
			if (_aboveMoistureBarriers[index])
			{
				return 0f;
			}
		}
		float num3 = num * _contaminationScaler;
		int num4 = originTerrainHeight - num2;
		if (num4 < 0)
		{
			return num3;
		}
		float num5 = (float)num4 * _verticalCostModifier;
		return num3 - num5;
	}

	private float GetContaminationFromNeighbor(int neighborIndex2D, int floor, int height, float currentContamination, float cost)
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
				float contaminationFromNeighbor = GetContaminationFromNeighbor(num, ceiling, height, cost);
				if (contaminationFromNeighbor > currentContamination)
				{
					currentContamination = contaminationFromNeighbor;
				}
			}
		}
		return currentContamination;
	}

	private float GetContaminationFromNeighbor(int neighborIndex3D, int neighborTerrainHeight, int originTerrainHeight, float spreadCost)
	{
		int num = originTerrainHeight - neighborTerrainHeight;
		if (num < 0)
		{
			num = 0;
		}
		float num2 = (float)num * _verticalCostModifier;
		return _lastTickContaminationCandidates[neighborIndex3D] - num2 - spreadCost;
	}
}
