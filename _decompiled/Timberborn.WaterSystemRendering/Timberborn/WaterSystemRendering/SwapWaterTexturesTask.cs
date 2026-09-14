using System;
using Timberborn.Multithreading;
using UnityEngine;

namespace Timberborn.WaterSystemRendering;

internal readonly struct SwapWaterTexturesTask(int maxColumnCount, bool anyColumnChanged, ColumnChangeTracker columnChangeTracker, ColumnCountTracker columnCountTracker, DataTextureArray<float> waterDepths, DataTextureArray<Vector2> outflows, DataTextureArray<byte> contaminations, DataTextureArray<Vector2> columns, DataTextureArray<Vector2> linkBarriers, DataTextureArray<float> flowLimits, bool[] tilesWithWater) : IParallelizerSingleTask
{
	private readonly int _maxColumnCount = maxColumnCount;

	private readonly bool _anyColumnChanged = anyColumnChanged;

	private readonly ColumnChangeTracker _columnChangeTracker = columnChangeTracker;

	private readonly ColumnCountTracker _columnCountTracker = columnCountTracker;

	private readonly DataTextureArray<float> _waterDepths = waterDepths;

	private readonly DataTextureArray<Vector2> _outflows = outflows;

	private readonly DataTextureArray<byte> _contaminations = contaminations;

	private readonly DataTextureArray<Vector2> _columns = columns;

	private readonly DataTextureArray<Vector2> _linkBarriers = linkBarriers;

	private readonly DataTextureArray<float> _flowLimits = flowLimits;

	private readonly bool[] _tilesWithWater = tilesWithWater;

	public void Run()
	{
		Array.Clear(_tilesWithWater, 0, _tilesWithWater.Length);
		_columnCountTracker.Update(_maxColumnCount);
		_columnChangeTracker.Update(_anyColumnChanged);
		_outflows.SwapDataAndClear(_columnCountTracker.MaxCount);
		_contaminations.SwapDataAndClear(_columnCountTracker.MaxCount);
		_waterDepths.SwapDataAndClear(_columnCountTracker.MaxCount);
		_flowLimits.SwapDataAndClear(_columnCountTracker.MaxCount);
		if (_columnChangeTracker.AnyColumnChanged())
		{
			_columns.SwapDataAndClear(_columnCountTracker.MaxCount);
			_linkBarriers.SwapDataAndClear(_columnCountTracker.MaxCount);
		}
	}
}
