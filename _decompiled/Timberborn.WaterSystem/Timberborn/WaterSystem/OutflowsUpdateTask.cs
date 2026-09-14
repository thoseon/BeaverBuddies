using System;
using System.Collections.Generic;
using Timberborn.Common;
using Timberborn.Multithreading;
using UnityEngine;

namespace Timberborn.WaterSystem;

internal readonly struct OutflowsUpdateTask(FlowLimitCalculator flowLimitCalculator, WaterFlowRetriever waterFlowRetriever, List<DirectedFlow>[] directedFlows, WaterFlow[] baseLevelFlows, ReadOnlyArray<byte> waterColumnCounts, ReadOnlyArray<WaterColumn> waterColumns, ReadOnlyArray<int> limitedDirections, ReadOnlyArray<float> heightLimits, ReadOnlyArray<float> inflowLimits, ReadOnlyArray<ColumnOutflows> outflows, int xMapSize, int stride, int verticalStride, float deltaTime, float overflowPressureFactor, float maxHardDamDecrease, float hardDamSmoothingFactor, float minHardDamSmoothing, float maxHardDamSmoothing, float hardDamOffset, float softDamOffset, float waterSpillThreshold, float waterFlowFactor) : IParallelizerLoopTask
{
	private readonly FlowLimitCalculator _flowLimitCalculator = flowLimitCalculator;

	private readonly WaterFlowRetriever _waterFlowRetriever = waterFlowRetriever;

	private readonly List<DirectedFlow>[] _directedFlows = directedFlows;

	private readonly WaterFlow[] _baseLevelFlows = baseLevelFlows;

	private readonly ReadOnlyArray<byte> _waterColumnCounts = waterColumnCounts;

	private readonly ReadOnlyArray<WaterColumn> _waterColumns = waterColumns;

	private readonly ReadOnlyArray<int> _limitedDirections = limitedDirections;

	private readonly ReadOnlyArray<float> _heightLimits = heightLimits;

	private readonly ReadOnlyArray<float> _inflowLimits = inflowLimits;

	private readonly ReadOnlyArray<ColumnOutflows> _outflows = outflows;

	private readonly int _xMapSize = xMapSize;

	private readonly int _stride = stride;

	private readonly int _verticalStride = verticalStride;

	private readonly float _deltaTime = deltaTime;

	private readonly float _overflowPressureFactor = overflowPressureFactor;

	private readonly float _maxHardDamDecrease = maxHardDamDecrease;

	private readonly float _hardDamSmoothingFactor = hardDamSmoothingFactor;

	private readonly float _minHardDamSmoothing = minHardDamSmoothing;

	private readonly float _maxHardDamSmoothing = maxHardDamSmoothing;

	private readonly float _hardDamOffset = hardDamOffset;

	private readonly float _softDamOffset = softDamOffset;

	private readonly float _waterSpillThreshold = waterSpillThreshold;

	private readonly float _waterFlowFactor = waterFlowFactor;

	public void Run(int y)
	{
		int num = (y + 1) * _stride;
		for (int i = 0; i < _xMapSize; i++)
		{
			int num2 = i + 1 + num;
			_directedFlows[num2].Clear();
			byte b = _waterColumnCounts[num2];
			for (int j = 0; j < b; j++)
			{
				int index3D = num2 + j * _verticalStride;
				UpdateOutflows(num2, index3D);
			}
		}
	}

	private void UpdateOutflows(int index, int index3D)
	{
		ref readonly WaterColumn reference = ref _waterColumns[index3D];
		float num = reference.WaterDepth + reference.Overflow;
		bool flag = index == index3D;
		if (num == 0f)
		{
			return;
		}
		ref WaterFlow reference2 = ref _baseLevelFlows[index];
		List<DirectedFlow> list = _directedFlows[index];
		int count = list.Count;
		float sumOfOutflows = 0f;
		Outflow(in reference, index, index3D, -_stride, ref reference2, ref sumOfOutflows);
		Outflow(in reference, index, index3D, -1, ref reference2, ref sumOfOutflows);
		Outflow(in reference, index, index3D, _stride, ref reference2, ref sumOfOutflows);
		Outflow(in reference, index, index3D, 1, ref reference2, ref sumOfOutflows);
		if (sumOfOutflows == 0f)
		{
			return;
		}
		float num2 = num / (sumOfOutflows * _deltaTime);
		if (num2 < 1f)
		{
			if (flag)
			{
				reference2.Bottom *= num2;
				reference2.Left *= num2;
				reference2.Top *= num2;
				reference2.Right *= num2;
			}
			for (int i = count; i < list.Count; i++)
			{
				list[i] = list[i].MultiplyFlow(num2);
			}
		}
	}

	private void Outflow(in WaterColumn waterColumn, int index, int index3D, int targetOffset, ref WaterFlow baseLevelFlow, ref float sumOfOutflows)
	{
		int num = index + targetOffset;
		for (int i = 0; i < _waterColumnCounts[num]; i++)
		{
			int num2 = num + i * _verticalStride;
			ref readonly WaterColumn reference = ref _waterColumns[num2];
			if (reference.Ceiling <= waterColumn.Floor)
			{
				continue;
			}
			if ((float)(int)reference.Floor >= (float)(int)waterColumn.Floor + waterColumn.WaterDepth)
			{
				break;
			}
			float num3 = GetOutflow(in waterColumn, index, index3D, in reference, num, num2);
			if (!(num3 > 0f))
			{
				continue;
			}
			float inflowLimit = _flowLimitCalculator.GetInflowLimit(_inflowLimits, num, reference.Floor);
			if (num3 > inflowLimit)
			{
				num3 = inflowLimit;
			}
			if (index == index3D && num == num2)
			{
				if (targetOffset == -_stride)
				{
					baseLevelFlow.Bottom = num3;
				}
				else if (targetOffset == -1)
				{
					baseLevelFlow.Left = num3;
				}
				else if (targetOffset == _stride)
				{
					baseLevelFlow.Top = num3;
				}
				else if (targetOffset == 1)
				{
					baseLevelFlow.Right = num3;
				}
			}
			else
			{
				_directedFlows[index].Add(new DirectedFlow(num3, num2, index3D));
			}
			sumOfOutflows += num3;
		}
	}

	private float GetOutflow(in WaterColumn originColumn, int origin, int index3D, in WaterColumn targetColumn, int target, int targetIndex3D)
	{
		int direction = target - origin;
		byte floor = originColumn.Floor;
		byte floor2 = targetColumn.Floor;
		if (!_flowLimitCalculator.CanInflowInDirection(_limitedDirections, target, floor2, direction) || !_flowLimitCalculator.CanOutflowInDirection(_limitedDirections, origin, floor, direction))
		{
			return 0f;
		}
		float overflowPressureFactor = _overflowPressureFactor;
		float num = (float)(int)floor + originColumn.WaterDepth;
		int waterHeight = (int)Math.Ceiling(num);
		float num2 = originColumn.Overflow * overflowPressureFactor;
		float num3 = (float)(int)floor2 + targetColumn.WaterDepth;
		float num4 = targetColumn.Overflow * overflowPressureFactor;
		byte waterBase = ((floor > floor2) ? floor : floor2);
		float num5 = num + num2 - (num3 + num4);
		float num10;
		if (num5 > 0f)
		{
			float num6 = (float)(int)targetColumn.Ceiling - num3;
			float num7 = num5 - num6;
			float num8 = ((num5 > num2) ? num2 : num5);
			float num9 = ((num8 > num7) ? num8 : num7);
			num10 = num5 - num9 + num9 / overflowPressureFactor;
		}
		else
		{
			float num11 = (float)(int)originColumn.Ceiling - num;
			float num12 = 0f - num5;
			float num13 = num12 - num11;
			float num14 = ((num12 < num4) ? num12 : num4);
			float num15 = ((num14 > num13) ? num14 : num13);
			num10 = num5 + num15 - num15 / overflowPressureFactor;
		}
		ref readonly ColumnOutflows outflows = ref _outflows[index3D];
		float num16 = _waterFlowRetriever.GetFlow(targetIndex3D, in outflows) * 0.999f;
		float heightLimit = _flowLimitCalculator.GetHeightLimit(_heightLimits, target, waterBase, waterHeight);
		if (heightLimit >= 0f)
		{
			num16 *= 0.995f;
			float num17 = num + num2 - (float)(int)floor2;
			if (num17 < heightLimit)
			{
				float num18 = Mathf.Clamp01((heightLimit - num17) / _hardDamOffset);
				float num19 = num - ((float)(int)floor + originColumn.OldWaterDepth);
				float num20 = Mathf.Clamp(1f - num19 * _hardDamSmoothingFactor, _minHardDamSmoothing, _maxHardDamSmoothing);
				float num21 = _maxHardDamDecrease * Mathf.Clamp01(num18 * num20);
				return num16 - num21;
			}
			float num22 = num17 - heightLimit;
			if (num22 < _softDamOffset && num10 > 0f)
			{
				num10 *= num22 / _softDamOffset;
			}
		}
		else if (targetColumn.WaterDepth + targetColumn.Overflow == 0f && floor == floor2)
		{
			num10 -= _waterSpillThreshold;
		}
		float num23 = _waterFlowFactor * num10;
		return num16 + num23;
	}
}
