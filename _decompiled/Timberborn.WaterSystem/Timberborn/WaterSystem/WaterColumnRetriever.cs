using Timberborn.Common;

namespace Timberborn.WaterSystem;

public class WaterColumnRetriever
{
	private readonly ReadOnlyWaterColumn _emptyWaterColumn;

	public ref readonly ReadOnlyWaterColumn GetColumn(ReadOnlyArray<byte> columnCounts, ReadOnlyArray<ReadOnlyWaterColumn> waterColumns, int verticalStride, int index, int height)
	{
		for (int i = 0; i < columnCounts[index]; i++)
		{
			ref readonly ReadOnlyWaterColumn reference = ref waterColumns[i * verticalStride + index];
			if (height < reference.Floor)
			{
				break;
			}
			if (height < reference.Ceiling)
			{
				return ref reference;
			}
		}
		return ref _emptyWaterColumn;
	}

	public bool TryGetColumnWithFloorAtHeight(in ReadOnlyArray<byte> columnCounts, in ReadOnlyArray<ReadOnlyWaterColumn> waterColumns, int verticalStride, int index2D, int height, out int index3D)
	{
		for (int i = 0; i < columnCounts[index2D]; i++)
		{
			index3D = i * verticalStride + index2D;
			byte floor = waterColumns[index3D].Floor;
			if (height == floor)
			{
				return true;
			}
			if (floor > height)
			{
				return false;
			}
		}
		index3D = 0;
		return false;
	}

	public bool TryGetColumnWithCeilingAtHeight(in ReadOnlyArray<byte> columnCounts, in ReadOnlyArray<ReadOnlyWaterColumn> waterColumns, int verticalStride, int index2D, int height, out int index3D)
	{
		for (int i = 0; i < columnCounts[index2D]; i++)
		{
			index3D = i * verticalStride + index2D;
			byte ceiling = waterColumns[index3D].Ceiling;
			if (height == ceiling)
			{
				return true;
			}
			if (ceiling > height)
			{
				return false;
			}
		}
		index3D = 0;
		return false;
	}

	public bool TryGetTopWateredColumn(in ReadOnlyArray<byte> columnCounts, in ReadOnlyArray<ReadOnlyWaterColumn> waterColumns, int verticalStride, int terrainHeight, int targetIndex2D, out int index3D)
	{
		for (int num = columnCounts[targetIndex2D] - 1; num >= 0; num--)
		{
			index3D = targetIndex2D + num * verticalStride;
			ref readonly ReadOnlyWaterColumn reference = ref waterColumns[index3D];
			if (reference.Floor <= terrainHeight && reference.WaterDepth > 0f)
			{
				return true;
			}
		}
		index3D = 0;
		return false;
	}

	public bool TryGetTopContaminatedColumn(in ReadOnlyArray<byte> columnCounts, in ReadOnlyArray<ReadOnlyWaterColumn> waterColumns, int verticalStride, int terrainHeight, int targetIndex2D, out int index3D)
	{
		for (int num = columnCounts[targetIndex2D] - 1; num >= 0; num--)
		{
			index3D = targetIndex2D + num * verticalStride;
			ref readonly ReadOnlyWaterColumn reference = ref waterColumns[index3D];
			if (reference.Floor <= terrainHeight && reference.Contamination > 0f)
			{
				return true;
			}
		}
		index3D = 0;
		return false;
	}
}
