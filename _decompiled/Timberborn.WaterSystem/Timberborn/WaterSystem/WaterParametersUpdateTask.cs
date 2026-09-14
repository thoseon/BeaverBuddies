using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.Common;
using Timberborn.Multithreading;

namespace Timberborn.WaterSystem;

internal readonly struct WaterParametersUpdateTask(WaterDepthSetter waterDepthSetter, WaterColumn[] waterColumns, ColumnOutflows[] columnOutflows, ReadOnlyArray<byte> waterColumnCounts, ImmutableArray<int> neighborOffsets, ReadOnlyArray<WaterFlow> baseLevelFlows, ReadOnlyArray<float> evaporationModifiers, ReadOnlyArray<List<DirectedFlow>> directedFlows, int xMapSize, int stride, int verticalStride, float deltaTime, float outflowBalancingScaler, float fastEvaporationDepthThreshold, float fastEvaporationSpeed, float normalEvaporationSpeed) : IParallelizerLoopTask
{
	private readonly WaterDepthSetter _waterDepthSetter = waterDepthSetter;

	private readonly WaterColumn[] _waterColumns = waterColumns;

	private readonly ColumnOutflows[] _columnOutflows = columnOutflows;

	private readonly ReadOnlyArray<byte> _waterColumnCounts = waterColumnCounts;

	private readonly ImmutableArray<int> _neighborOffsets = neighborOffsets;

	private readonly ReadOnlyArray<WaterFlow> _baseLevelFlows = baseLevelFlows;

	private readonly ReadOnlyArray<float> _evaporationModifiers = evaporationModifiers;

	private readonly ReadOnlyArray<List<DirectedFlow>> _directedFlows = directedFlows;

	private readonly int _xMapSize = xMapSize;

	private readonly int _stride = stride;

	private readonly int _verticalStride = verticalStride;

	private readonly float _deltaTime = deltaTime;

	private readonly float _outflowBalancingScaler = outflowBalancingScaler;

	private readonly float _fastEvaporationDepthThreshold = fastEvaporationDepthThreshold;

	private readonly float _fastEvaporationSpeed = fastEvaporationSpeed;

	private readonly float _normalEvaporationSpeed = normalEvaporationSpeed;

	public void Run(int y)
	{
		int num = (y + 1) * _stride;
		for (int i = 0; i < _xMapSize; i++)
		{
			int num2 = i + 1 + num;
			for (int j = 0; j < _waterColumnCounts[num2]; j++)
			{
				int num3 = num2 + j * _verticalStride;
				ref WaterColumn waterColumn = ref _waterColumns[num3];
				float waterDepthChange = ProcessWaterDepthChanges(in waterColumn, num2, num3);
				_waterDepthSetter.SetWaterDepth(waterDepthChange, ref waterColumn);
			}
		}
	}

	private float ProcessWaterDepthChanges(in WaterColumn waterColumn, int index, int index3D)
	{
		int num = index - _stride;
		int num2 = index - 1;
		int num3 = index + _stride;
		int num4 = index + 1;
		bool flag = index == index3D;
		float num5 = 0f;
		float outflowBalancingScaler = _outflowBalancingScaler;
		ref ColumnOutflows reference = ref _columnOutflows[index3D];
		reference.BottomFlow.Flow = 0f;
		reference.BottomFlow.Index3D = -1;
		reference.LeftFlow.Flow = 0f;
		reference.LeftFlow.Index3D = -1;
		reference.TopFlow.Flow = 0f;
		reference.TopFlow.Index3D = -1;
		reference.RightFlow.Flow = 0f;
		reference.RightFlow.Index3D = -1;
		reference.Outflows?.Clear();
		if (flag)
		{
			ref readonly WaterFlow reference2 = ref _baseLevelFlows[index];
			float bottom = reference2.Bottom;
			float tempBaseLevelFlow = GetTempBaseLevelFlow(num, index3D);
			num5 += tempBaseLevelFlow - bottom;
			if (bottom > 0f)
			{
				float num6 = bottom - tempBaseLevelFlow * outflowBalancingScaler;
				if (num6 > 0f)
				{
					reference.BottomFlow.Flow = num6;
					reference.BottomFlow.Index3D = num;
				}
			}
			float left = reference2.Left;
			float tempBaseLevelFlow2 = GetTempBaseLevelFlow(num2, index3D);
			num5 += tempBaseLevelFlow2 - left;
			if (left > 0f)
			{
				float num7 = left - tempBaseLevelFlow2 * outflowBalancingScaler;
				if (num7 > 0f)
				{
					reference.LeftFlow.Flow = num7;
					reference.LeftFlow.Index3D = num2;
				}
			}
			float top = reference2.Top;
			float tempBaseLevelFlow3 = GetTempBaseLevelFlow(num3, index3D);
			num5 += tempBaseLevelFlow3 - top;
			if (top > 0f)
			{
				float num8 = top - tempBaseLevelFlow3 * outflowBalancingScaler;
				if (num8 > 0f)
				{
					reference.TopFlow.Flow = num8;
					reference.TopFlow.Index3D = num3;
				}
			}
			float right = reference2.Right;
			float tempBaseLevelFlow4 = GetTempBaseLevelFlow(num4, index3D);
			num5 += tempBaseLevelFlow4 - right;
			if (right > 0f)
			{
				float num9 = right - tempBaseLevelFlow4 * outflowBalancingScaler;
				if (num9 > 0f)
				{
					reference.RightFlow.Flow = num9;
					reference.RightFlow.Index3D = num4;
				}
			}
		}
		for (int i = 0; i < _neighborOffsets.Length; i++)
		{
			int num10 = _neighborOffsets[i];
			int num11 = index + num10;
			for (int j = (flag ? 1 : 0); j < _waterColumnCounts[num11]; j++)
			{
				int num12 = num11 + j * _verticalStride;
				float tempFlow = GetTempFlow(index, index3D, num12);
				float tempFlow2 = GetTempFlow(num11, num12, index3D);
				num5 += tempFlow2 - tempFlow;
				float num13 = tempFlow - tempFlow2 * outflowBalancingScaler;
				if (!(num13 > 0f))
				{
					continue;
				}
				if (num10 == -_stride)
				{
					if (reference.BottomFlow.Index3D == -1)
					{
						reference.BottomFlow.Flow = num13;
						reference.BottomFlow.Index3D = num12;
						continue;
					}
					if (reference.BottomFlow.Index3D == num12)
					{
						reference.BottomFlow.Flow += num13;
						continue;
					}
					ref List<TargetedFlow> outflows = ref reference.Outflows;
					if (outflows == null)
					{
						outflows = new List<TargetedFlow>();
					}
					reference.Outflows.Add(new TargetedFlow(num13, num12));
				}
				else if (num10 == -1)
				{
					if (reference.LeftFlow.Index3D == -1)
					{
						reference.LeftFlow.Flow = num13;
						reference.LeftFlow.Index3D = num12;
						continue;
					}
					if (reference.LeftFlow.Index3D == num12)
					{
						reference.LeftFlow.Flow += num13;
						continue;
					}
					ref List<TargetedFlow> outflows = ref reference.Outflows;
					if (outflows == null)
					{
						outflows = new List<TargetedFlow>();
					}
					reference.Outflows.Add(new TargetedFlow(num13, num12));
				}
				else if (num10 == _stride)
				{
					if (reference.TopFlow.Index3D == -1)
					{
						reference.TopFlow.Flow = num13;
						reference.TopFlow.Index3D = num12;
						continue;
					}
					if (reference.TopFlow.Index3D == num12)
					{
						reference.TopFlow.Flow += num13;
						continue;
					}
					ref List<TargetedFlow> outflows = ref reference.Outflows;
					if (outflows == null)
					{
						outflows = new List<TargetedFlow>();
					}
					reference.Outflows.Add(new TargetedFlow(num13, num12));
				}
				else
				{
					if (num10 != 1)
					{
						continue;
					}
					if (reference.RightFlow.Index3D == -1)
					{
						reference.RightFlow.Flow = num13;
						reference.RightFlow.Index3D = num12;
						continue;
					}
					if (reference.RightFlow.Index3D == num12)
					{
						reference.RightFlow.Flow += num13;
						continue;
					}
					ref List<TargetedFlow> outflows = ref reference.Outflows;
					if (outflows == null)
					{
						outflows = new List<TargetedFlow>();
					}
					reference.Outflows.Add(new TargetedFlow(num13, num12));
				}
			}
		}
		float num14 = ((waterColumn.WaterDepth < _fastEvaporationDepthThreshold) ? _fastEvaporationSpeed : _normalEvaporationSpeed);
		float num15 = _evaporationModifiers[index3D];
		float num16 = num14 * num15;
		return (num5 - num16) * _deltaTime;
	}

	private float GetTempBaseLevelFlow(int originIndex3D, int targetIndex3D)
	{
		ref readonly WaterFlow reference = ref _baseLevelFlows[originIndex3D];
		int num = originIndex3D - targetIndex3D;
		if (num == -_stride)
		{
			return reference.Top;
		}
		if (num == -1)
		{
			return reference.Right;
		}
		if (num == _stride)
		{
			return reference.Bottom;
		}
		if (num == 1)
		{
			return reference.Left;
		}
		throw new ArgumentOutOfRangeException();
	}

	private float GetTempFlow(int originIndex, int originIndex3D, int targetIndex3D)
	{
		float num = 0f;
		List<DirectedFlow> list = _directedFlows[originIndex];
		for (int i = 0; i < list.Count; i++)
		{
			DirectedFlow directedFlow = list[i];
			if (directedFlow.OriginIndex3D == originIndex3D && directedFlow.TargetIndex3D == targetIndex3D)
			{
				num += directedFlow.Flow;
			}
		}
		return num;
	}
}
