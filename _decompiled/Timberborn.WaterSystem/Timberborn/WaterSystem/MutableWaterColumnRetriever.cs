using System;

namespace Timberborn.WaterSystem;

internal class MutableWaterColumnRetriever
{
	public ref WaterColumn GetColumn(ReadOnlySpan<byte> columnCounts, Span<WaterColumn> waterColumns, int verticalStride, int index, int height)
	{
		for (int i = 0; i < columnCounts[index]; i++)
		{
			ref WaterColumn reference = ref waterColumns[i * verticalStride + index];
			if (height < reference.Floor)
			{
				break;
			}
			if (height < reference.Ceiling)
			{
				return ref reference;
			}
		}
		throw new InvalidOperationException($"Column for index {index} and height {height} not found");
	}

	public bool TryGetColumnIndex(ReadOnlySpan<byte> columnCounts, Span<WaterColumn> waterColumns, int verticalStride, int index, int height, out int columnIndex)
	{
		for (int i = 0; i < columnCounts[index]; i++)
		{
			columnIndex = i * verticalStride + index;
			ref WaterColumn reference = ref waterColumns[columnIndex];
			if (height < reference.Floor)
			{
				break;
			}
			if (height < reference.Ceiling)
			{
				return true;
			}
		}
		columnIndex = -1;
		return false;
	}
}
