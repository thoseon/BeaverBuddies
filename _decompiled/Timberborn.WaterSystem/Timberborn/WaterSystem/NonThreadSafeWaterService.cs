using System;

namespace Timberborn.WaterSystem;

internal class NonThreadSafeWaterService : INonThreadSafeWaterService
{
	private readonly WaterSimulator _waterSimulator;

	private readonly ThreadSafeWaterMap _threadSafeWaterMap;

	private ColumnOutflows[] _nonThreadSafeOutflows = Array.Empty<ColumnOutflows>();

	public NonThreadSafeWaterService(WaterSimulator waterSimulator, ThreadSafeWaterMap threadSafeWaterMap)
	{
		_waterSimulator = waterSimulator;
		_threadSafeWaterMap = threadSafeWaterMap;
	}

	public void UpdateOutflowsData()
	{
		ReadOnlySpan<ColumnOutflows> outflows = _waterSimulator.Outflows;
		Array.Resize(ref _nonThreadSafeOutflows, outflows.Length);
		outflows.CopyTo(_nonThreadSafeOutflows);
	}

	public ReadOnlyWaterColumn GetColumnByIndex(int index3D)
	{
		return _threadSafeWaterMap.WaterColumns[index3D];
	}

	public ReadOnlyColumnOutflows ColumnOutflows(int index3D)
	{
		ref ColumnOutflows reference = ref _nonThreadSafeOutflows[index3D];
		return new ReadOnlyColumnOutflows(reference.BottomFlow, reference.LeftFlow, reference.TopFlow, reference.RightFlow, reference.Outflows);
	}

	public int GetColumnCount(int index)
	{
		return _threadSafeWaterMap.ColumnCounts[index];
	}
}
