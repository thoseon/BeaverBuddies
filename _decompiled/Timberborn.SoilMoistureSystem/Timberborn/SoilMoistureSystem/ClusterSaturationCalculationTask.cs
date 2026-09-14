using Timberborn.Common;
using Timberborn.Multithreading;
using Timberborn.WaterSystem;

namespace Timberborn.SoilMoistureSystem;

internal readonly struct ClusterSaturationCalculationTask(byte[] clusterSaturations, in ReadOnlyArray<byte> wateredNeighbours, in ReadOnlyArray<byte> columnCounts, in ReadOnlyArray<ReadOnlyWaterColumn> waterColumns, int maxClusterSaturation, int xMapSize, int stride, int verticalStride) : IParallelizerLoopTask
{
	private readonly byte[] _clusterSaturations = clusterSaturations;

	private readonly ReadOnlyArray<byte> _wateredNeighbours = wateredNeighbours;

	private readonly ReadOnlyArray<byte> _columnCounts = columnCounts;

	private readonly ReadOnlyArray<ReadOnlyWaterColumn> _waterColumns = waterColumns;

	private readonly int _maxClusterSaturation = maxClusterSaturation;

	private readonly int _xMapSize = xMapSize;

	private readonly int _stride = stride;

	private readonly int _verticalStride = verticalStride;

	public void Run(int y)
	{
		int num = (y + 1) * _stride;
		for (int i = 0; i < _xMapSize; i++)
		{
			int num2 = i + 1 + num;
			byte b = _columnCounts[num2];
			for (int j = 0; j < b; j++)
			{
				int num3 = num2 + j * _verticalStride;
				ref readonly ReadOnlyWaterColumn reference = ref _waterColumns[num3];
				if (reference.WaterDepth > 0f)
				{
					byte floor = reference.Floor;
					byte ceiling = reference.Ceiling;
					int index = num2 - _stride;
					int index2 = num2 - 1;
					int index3 = num2 + _stride;
					int index4 = num2 + 1;
					int num4 = _wateredNeighbours[num3];
					int wateredNeighbors = GetWateredNeighbors(index, floor, ceiling);
					int wateredNeighbors2 = GetWateredNeighbors(index2, floor, ceiling);
					int wateredNeighbors3 = GetWateredNeighbors(index3, floor, ceiling);
					int wateredNeighbors4 = GetWateredNeighbors(index4, floor, ceiling);
					if (wateredNeighbors > num4)
					{
						num4 = wateredNeighbors - 1;
					}
					if (wateredNeighbors2 > num4)
					{
						num4 = wateredNeighbors2 - 1;
					}
					if (wateredNeighbors3 > num4)
					{
						num4 = wateredNeighbors3 - 1;
					}
					if (wateredNeighbors4 > num4)
					{
						num4 = wateredNeighbors4 - 1;
					}
					int num5 = ((num4 > _maxClusterSaturation) ? _maxClusterSaturation : num4);
					_clusterSaturations[num3] = (byte)num5;
				}
				else
				{
					_clusterSaturations[num3] = 0;
				}
			}
		}
	}

	private int GetWateredNeighbors(int index, int floor, int ceiling)
	{
		byte b = _columnCounts[index];
		int num = 0;
		for (int i = 0; i < b; i++)
		{
			int index2 = index + i * _verticalStride;
			byte b2 = _wateredNeighbours[index2];
			if (b2 > num)
			{
				ref readonly ReadOnlyWaterColumn reference = ref _waterColumns[index2];
				if (reference.Ceiling > floor && reference.Floor < ceiling)
				{
					num = b2;
				}
			}
		}
		return num;
	}
}
