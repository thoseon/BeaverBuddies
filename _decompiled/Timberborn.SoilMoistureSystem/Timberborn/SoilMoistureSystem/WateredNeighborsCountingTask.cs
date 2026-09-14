using Timberborn.Common;
using Timberborn.Multithreading;
using Timberborn.WaterSystem;

namespace Timberborn.SoilMoistureSystem;

internal readonly struct WateredNeighborsCountingTask(byte[] wateredNeighbours, in ReadOnlyArray<byte> columnCounts, in ReadOnlyArray<ReadOnlyWaterColumn> waterColumns, int stride, int verticalStride, int xMapSize) : IParallelizerLoopTask
{
	private readonly byte[] _wateredNeighbours = wateredNeighbours;

	private readonly ReadOnlyArray<byte> _columnCounts = columnCounts;

	private readonly ReadOnlyArray<ReadOnlyWaterColumn> _waterColumns = waterColumns;

	private readonly int _stride = stride;

	private readonly int _verticalStride = verticalStride;

	private readonly int _xMapSize = xMapSize;

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
				if (_waterColumns[num3].WaterDepth > 0f)
				{
					int num4 = CountWateredNeighbors(num2, num3);
					_wateredNeighbours[num3] = (byte)(num4 + 1);
				}
				else
				{
					_wateredNeighbours[num3] = 0;
				}
			}
		}
	}

	private int CountWateredNeighbors(int index, int index3D)
	{
		ref readonly ReadOnlyWaterColumn originColumn = ref _waterColumns[index3D];
		return 0 + IsNeighborWatered(in originColumn, index - _stride - 1) + IsNeighborWatered(in originColumn, index - _stride) + IsNeighborWatered(in originColumn, index - _stride + 1) + IsNeighborWatered(in originColumn, index - 1) + IsNeighborWatered(in originColumn, index + 1) + IsNeighborWatered(in originColumn, index + _stride - 1) + IsNeighborWatered(in originColumn, index + _stride) + IsNeighborWatered(in originColumn, index + _stride + 1);
	}

	private int IsNeighborWatered(in ReadOnlyWaterColumn originColumn, int targetIndex2D)
	{
		byte floor = originColumn.Floor;
		byte ceiling = originColumn.Ceiling;
		ref readonly ReadOnlyWaterColumn reference = ref _waterColumns[targetIndex2D];
		if (reference.Floor >= floor)
		{
			if (ceiling > reference.Floor && reference.WaterDepth > 0f)
			{
				return 1;
			}
			return 0;
		}
		for (int num = _columnCounts[targetIndex2D] - 1; num >= 0; num--)
		{
			int index = targetIndex2D + num * _verticalStride;
			ref readonly ReadOnlyWaterColumn reference2 = ref _waterColumns[index];
			if (reference2.Floor < ceiling && reference2.Ceiling > floor && reference2.WaterDepth > 0f)
			{
				return 1;
			}
		}
		return 0;
	}
}
