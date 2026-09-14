using System.Collections.Generic;
using Timberborn.Common;
using Timberborn.Multithreading;

namespace Timberborn.WaterSystem;

internal readonly struct SimulateContaminationTask(FlowLimitCalculator flowLimitCalculator, WaterFlowRetriever waterFlowRetriever, float[] contaminationsBuffer, Diffusions[] baseLevelDiffusions, byte[] targetedDiffusionCount, List<TargetedDiffusion>[] targetedDiffusions, ReadOnlyArray<byte> waterColumnCounts, ReadOnlyArray<WaterColumn> waterColumns, ReadOnlyArray<ColumnOutflows> outflows, ReadOnlyArray<int> limitedDirections, ReadOnlyArray<float> heightLimits, int xMapSize, int stride, int verticalStride, float deltaTime, float overflowPressureFactor, float maxWaterContamination, double diffusionOutflowLimit, double diffusionDepthLimit) : IParallelizerLoopTask
{
	private readonly FlowLimitCalculator _flowLimitCalculator = flowLimitCalculator;

	private readonly WaterFlowRetriever _waterFlowRetriever = waterFlowRetriever;

	private readonly float[] _contaminationsBuffer = contaminationsBuffer;

	private readonly Diffusions[] _baseLevelDiffusions = baseLevelDiffusions;

	private readonly byte[] _targetedDiffusionCount = targetedDiffusionCount;

	private readonly List<TargetedDiffusion>[] _targetedDiffusions = targetedDiffusions;

	private readonly ReadOnlyArray<byte> _waterColumnCounts = waterColumnCounts;

	private readonly ReadOnlyArray<WaterColumn> _waterColumns = waterColumns;

	private readonly ReadOnlyArray<ColumnOutflows> _outflows = outflows;

	private readonly ReadOnlyArray<int> _limitedDirections = limitedDirections;

	private readonly ReadOnlyArray<float> _heightLimits = heightLimits;

	private readonly int _xMapSize = xMapSize;

	private readonly int _stride = stride;

	private readonly int _verticalStride = verticalStride;

	private readonly float _deltaTime = deltaTime;

	private readonly float _overflowPressureFactor = overflowPressureFactor;

	private readonly float _maxWaterContamination = maxWaterContamination;

	private readonly double _diffusionOutflowLimit = diffusionOutflowLimit;

	private readonly double _diffusionDepthLimit = diffusionDepthLimit;

	public void Run(int y)
	{
		int num = (y + 1) * _stride;
		for (int i = 0; i < _xMapSize; i++)
		{
			int num2 = i + 1 + num;
			_targetedDiffusions[num2].Clear();
			for (int j = 0; j < _waterColumnCounts[num2]; j++)
			{
				int num3 = num2 + j * _verticalStride;
				ref readonly WaterColumn reference = ref _waterColumns[num3];
				if (reference.WaterDepth > 0f)
				{
					SimulateContamination(in reference, num2, num3);
				}
			}
		}
	}

	private void SimulateContamination(in WaterColumn waterColumn, int index, int index3D)
	{
		ref readonly ColumnOutflows outflows = ref _outflows[index3D];
		float waterReceived = 0f;
		float waterDisposed = 0f;
		float contaminationChange = 0f;
		ref Diffusions baseLevelDiffusion = ref _baseLevelDiffusions[index];
		CalculateContaminationChange(in waterColumn, index, index3D, -_stride, in outflows, ref baseLevelDiffusion, ref waterReceived, ref waterDisposed, ref contaminationChange);
		CalculateContaminationChange(in waterColumn, index, index3D, -1, in outflows, ref baseLevelDiffusion, ref waterReceived, ref waterDisposed, ref contaminationChange);
		CalculateContaminationChange(in waterColumn, index, index3D, _stride, in outflows, ref baseLevelDiffusion, ref waterReceived, ref waterDisposed, ref contaminationChange);
		CalculateContaminationChange(in waterColumn, index, index3D, 1, in outflows, ref baseLevelDiffusion, ref waterReceived, ref waterDisposed, ref contaminationChange);
		if (waterReceived > 0f)
		{
			float num = waterColumn.WaterDepth + waterColumn.Overflow * _overflowPressureFactor;
			float num2 = num - (waterDisposed + waterReceived);
			float num3 = (waterColumn.Contamination * (num2 + waterDisposed) + contaminationChange) / num;
			if (num3 < 0f)
			{
				_contaminationsBuffer[index3D] = 0f;
			}
			else
			{
				_contaminationsBuffer[index3D] = ((num3 > _maxWaterContamination) ? _maxWaterContamination : num3);
			}
		}
		else
		{
			_contaminationsBuffer[index3D] = waterColumn.Contamination;
		}
	}

	private void CalculateContaminationChange(in WaterColumn waterColumn, int index, int index3D, int targetOffset, in ColumnOutflows outflows, ref Diffusions baseLevelDiffusion, ref float waterReceived, ref float waterDisposed, ref float contaminationChange)
	{
		int num = index + targetOffset;
		byte b = _waterColumnCounts[num];
		for (int i = 0; i < b; i++)
		{
			int num2 = num + i * _verticalStride;
			float flow = _waterFlowRetriever.GetFlow(num2, in outflows);
			ref readonly ColumnOutflows outflows2 = ref _outflows[num2];
			float num3 = _waterFlowRetriever.GetFlow(index3D, in outflows2) - flow;
			if (num3 == 0f)
			{
				continue;
			}
			float num4 = num3 * _deltaTime;
			if (num4 > 0f)
			{
				waterReceived += num4;
				contaminationChange += num4 * _waterColumns[num2].Contamination;
			}
			else
			{
				waterDisposed += num4;
			}
			if (!CanDiffuse(in waterColumn, index, index3D, num, num2, num3, targetOffset))
			{
				continue;
			}
			if (index == index3D && num == num2)
			{
				if (targetOffset == -_stride)
				{
					baseLevelDiffusion.Bottom = true;
				}
				else if (targetOffset == -1)
				{
					baseLevelDiffusion.Left = true;
				}
				else if (targetOffset == _stride)
				{
					baseLevelDiffusion.Top = true;
				}
				else if (targetOffset == 1)
				{
					baseLevelDiffusion.Right = true;
				}
			}
			else
			{
				_targetedDiffusions[index].Add(new TargetedDiffusion(num2, index3D));
			}
			_targetedDiffusionCount[index3D]++;
		}
	}

	private bool CanDiffuse(in WaterColumn waterColumn, int index, int index3D, int target, int target3D, float netFlowToTarget, int targetOffset)
	{
		if ((double)netFlowToTarget >= _diffusionOutflowLimit || (double)netFlowToTarget <= 0.0 - _diffusionOutflowLimit)
		{
			return false;
		}
		float waterDepth = waterColumn.WaterDepth;
		byte floor = waterColumn.Floor;
		if (waterDepth <= 1f && _flowLimitCalculator.HasInflowLimit(_heightLimits, index, floor))
		{
			return false;
		}
		ref readonly WaterColumn reference = ref _waterColumns[target3D];
		float waterDepth2 = reference.WaterDepth;
		if (waterDepth2 <= 0f)
		{
			return false;
		}
		byte floor2 = reference.Floor;
		if (waterDepth2 <= 1f && _flowLimitCalculator.HasInflowLimit(_heightLimits, target, floor2))
		{
			return false;
		}
		if (!_flowLimitCalculator.CanInflowInDirection(_limitedDirections, target, floor2, targetOffset) || !_flowLimitCalculator.CanInflowInDirection(_limitedDirections, index, floor, -targetOffset))
		{
			return false;
		}
		float num = waterDepth2 + (float)(int)floor2 + reference.Overflow * _overflowPressureFactor;
		float num2 = waterDepth + (float)(int)floor + waterColumn.Overflow * _overflowPressureFactor;
		if ((double)((num > num2) ? (num - num2) : (num2 - num)) > _diffusionDepthLimit)
		{
			return false;
		}
		return true;
	}
}
