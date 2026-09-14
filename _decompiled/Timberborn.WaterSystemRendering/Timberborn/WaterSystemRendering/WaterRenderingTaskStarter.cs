using Timberborn.Common;
using Timberborn.MapIndexSystem;
using Timberborn.Multithreading;
using Timberborn.SingletonSystem;
using Timberborn.TerrainSystem;
using Timberborn.WaterSystem;
using UnityEngine;

namespace Timberborn.WaterSystemRendering;

internal class WaterRenderingTaskStarter : ILoadableSingleton
{
	private readonly IParallelizer _parallelizer;

	private readonly MapIndexService _mapIndexService;

	private int _stride;

	private int _verticalStride;

	private Vector3Int _mapSize;

	private Vector2Int _tileCount;

	public WaterRenderingTaskStarter(IParallelizer parallelizer, MapIndexService mapIndexService)
	{
		_parallelizer = parallelizer;
		_mapIndexService = mapIndexService;
	}

	public void Load()
	{
		_stride = _mapIndexService.Stride;
		_verticalStride = _mapIndexService.VerticalStride;
		_mapSize = _mapIndexService.TerrainSize;
		_tileCount = WorldTiling.TileCount2D(_mapSize.x, _mapSize.y);
	}

	public void StartTask(bool runSynchronously, int maxColumnCount, bool anyColumnChanged, ColumnChangeTracker columnChangeTracker, ColumnCountTracker columnCountTracker, DataTextureArray<float> waterDepths, DataTextureArray<Vector2> outflows, DataTextureArray<byte> contaminations, DataTextureArray<Vector2> columns, DataTextureArray<Vector2> linkBarriers, DataTextureArray<float> flowLimits, bool[] tilesWithWater, in ReadOnlyJaggedArray<float> flowLimitsBuffer, in ReadOnlyArray<byte> columnCounts, in ReadOnlyArray<ReadOnlyWaterColumn> waterColumns, in ReadOnlyArray<Vector2> flowDirections, in ReadOnlyArray<int> limitedDirections)
	{
		SwapWaterTexturesTask task = new SwapWaterTexturesTask(maxColumnCount, anyColumnChanged, columnChangeTracker, columnCountTracker, waterDepths, outflows, contaminations, columns, linkBarriers, flowLimits, tilesWithWater);
		UpdateWaterTexturesTask task2 = new UpdateWaterTexturesTask(columnChangeTracker, _stride, _verticalStride, _mapSize, _tileCount, waterDepths, outflows, contaminations, columns, linkBarriers, flowLimits, tilesWithWater, in columnCounts, in waterColumns, in flowDirections, in limitedDirections, in flowLimitsBuffer);
		if (runSynchronously)
		{
			task.Run();
			for (int i = 0; i < _mapSize.y; i++)
			{
				task2.Run(i);
			}
		}
		else
		{
			_parallelizer.Schedule(in task).ContinueWith(0, _mapSize.y, 5, in task2);
		}
	}
}
